using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Mapping;
using webfmis.Models;
using webfmis.Services;

namespace webfmis.Controllers
{
    [Authorize]
    public class MstAccountController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstAccount");

            return View();
        }

        public IActionResult AccountList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstAccount
                    .OrderBy(x => x.Account)
                    .ToDataSourceResult(request);
            }

            return Json(result);
        }

        public IActionResult AccountArticleTypeList([DataSourceRequest] DataSourceRequest request, int AccountId)
        {
            List<MstAccountArticleType> result = null;

            using (var context = new FMISContext())
            {
                result = context.MstAccountArticleType
                    .Where(x => x.AccountId == AccountId)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult Add()
        {
            var result = new MstAccount()
            {
                Id = 0,
                AccountCode = "NA",
                Account = "NA",
                AccountTypeId = 47,
                AccountCashFlowId = null,
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> AccountDetail(int Id)
        {
            MstAccount result = null;

            using (var context = new FMISContext())
            {
                result = await context.MstAccount.FindAsync(Id);

                ViewData["ArticleTypes"] = context.MstArticleType.ToList();
            }

            ViewData["AccountId"] = Id;

            return PartialView("Detail", result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MstAccount data, List<MstAccountArticleType> dataSub)
        {
            data.MstAccountArticleType = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForMstAccount();
            var account = new MstAccount();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    account = await context.MstAccount.FindAsync(data.Id);

                    foreach (var line in data.MstAccountArticleType)
                    {
                        MstAccountArticleTypeDefaults(line);
                    }

                    mappingProfile.mapper.Map(data, account);
                    MstAccountDefaults(account);

                    await context.MstAccountArticleType.AddRangeAsync(account.MstAccountArticleType.Where(x => x.Id == 0));
                    context.MstAccountArticleType.RemoveRange(account.MstAccountArticleType.Where(x => x.IsDeleted));
                }
                else
                {
                    foreach (var line in data.MstAccountArticleType)
                    {
                        MstAccountArticleTypeDefaults(line);
                    }

                    account = mappingProfile.mapper.Map<MstAccount>(data);
                    MstAccountDefaults(account);

                    await context.MstAccount.AddAsync(account);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = account.Id, Account = account.Account });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = Common.GetAccountById(Id);

                var account = context.MstAccount
                    .Include(x => x.MstAccountArticleType)
                    .FirstOrDefault(x => x.Id == Id);

                context.MstAccount.Remove(account);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new MstAccount();

            using (var context = new FMISContext())
            {
                var account = Common.GetAccountById(Id);

                result = context.MstAccount
                    .Where(x => x.Account.CompareTo(account.Account) < 0)
                    .OrderByDescending(x => x.Account)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new MstAccount();

            using (var context = new FMISContext())
            {
                var account = Common.GetAccountById(Id);

                result = context.MstAccount
                    .Where(x => x.Account.CompareTo(account.Account) > 0)
                    .OrderBy(x => x.Account)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        private static void MstAccountDefaults(MstAccount account)
        {
            account.IsLocked = true;
            account.CreatedById = 1;
            account.CreatedDateTime = DateTime.Now;
            account.UpdatedById = 1;
            account.UpdatedDateTime = DateTime.Now;
        }

        private static void MstAccountArticleTypeDefaults(MstAccountArticleType accountArticleType)
        {

        }

        public IActionResult IsLockedFilterData() 
        {
            var result = new List<bool?>() { true, false, null };

            return Json(result);
        }

        public async Task<IActionResult> AccountTypes([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstAccountType>();

            using (var context = new FMISContext()) 
            {
                result = await context.MstAccountType
                    .OrderBy(x => x.AccountType)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> AccountCashFlows([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstAccountCashFlow>();

            using (var context = new FMISContext())
            {
                result = await context.MstAccountCashFlow
                    .OrderBy(x => x.AccountCashFlow)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> ArticleTypes()
        {
            var result = new List<MstArticleType>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticleType
                    .OrderBy(x => x.ArticleType)
                    .ToListAsync();
            }

            return Json(result);
        }
    }
}
