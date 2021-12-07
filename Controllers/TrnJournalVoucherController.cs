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
using webfmis.Utils;

namespace webfmis.Controllers
{
    [Authorize]
    public class TrnJournalVoucherController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnJournalVoucher");

            return View();
        }

        #region Grid
        public IActionResult JournalVoucherList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnJournalVoucher
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnJournalVoucher>().OrderByDescending(x => x.Jvnumber);
            }

            return Json(result);
        }

        public IActionResult TrnJournalVoucherLineList([DataSourceRequest] DataSourceRequest request, int JVId)
        {
            List<TrnJournalVoucherLine> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnJournalVoucherLine
                    .Where(x => x.Jvid == JVId)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.JVMaxCode(settings.BranchId);
            var jvCode = "";

            if (code != "0000000000" && code != null)
            {
                jvCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                jvCode = "0000000001";
            }

            var result = new TrnJournalVoucher()
            {
                BranchId = External.ReadSettings().Id,
                Jvnumber = jvCode,
                ManualJvnumber = jvCode,
                Jvdate = DateTime.Now.Date,
                Particulars = "NA",
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> JournalVoucherDetail(int Id)
        {
            TrnJournalVoucher result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnJournalVoucher.FindAsync(Id);
            }

            ViewData["JVId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddJVLine(int jvId)
        {
            var result = new TrnJournalVoucherLine()
            {
                Id = 0,
                Jvid = jvId,
                BranchId = External.ReadSettings().Id,
                AccountId = Common.ArticleAccountId(),           
                ArticleId = TransactionCommon.DefaultJVArticleId(Common.ArticleAccountId()),           
                Particulars = "NA",
                Aprrid = TransactionCommon.DefaultRRId(External.ReadSettings().Id),
                Arsiid = TransactionCommon.DefaultSIId(External.ReadSettings().Id),
                DebitAmount = 0,
                CreditAmount = 0
            };

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnJournalVoucher();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetJVById(Id);

                result = context.TrnJournalVoucher
                    .Where(x => x.Jvnumber.CompareTo(entity.Jvnumber) < 0)
                    .OrderByDescending(x => x.Jvnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnJournalVoucher();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetJVById(Id);

                result = context.TrnJournalVoucher
                    .Where(x => x.Jvnumber.CompareTo(entity.Jvnumber) > 0)
                    .OrderBy(x => x.Jvnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TrnJournalVoucher data, List<TrnJournalVoucherLine> dataSub)
        {
            data.TrnJournalVoucherLine = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();

            var mappingProfile = new MappingProfileForTrnJournalVoucher();
            var entity = new TrnJournalVoucher();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnJournalVoucher.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnJournalVoucherLine.AddRangeAsync(entity.TrnJournalVoucherLine.Where(x => x.Id == 0));
                    context.TrnJournalVoucherLine.RemoveRange(entity.TrnJournalVoucherLine.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnJournalVoucher>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnJournalVoucher.AddAsync(entity);
                }

                //if (entity.TrnJournalVoucherLine.Sum(x => x.DebitAmount) != entity.TrnJournalVoucherLine.Sum(x => x.CreditAmount))
                //{
                //    throw new InvalidOperationException("Balances are not equal!");
                //}

                await context.SaveChangesAsync();

                await Library.Journal.InsertJVJournal(entity);

                foreach (var item in entity.TrnJournalVoucherLine.Where(x => x.Aprrid != null)) 
                {
                    Library.Balances.UpdateAP((int)item.Aprrid);
                }
            }

            return Json(new { Id = entity.Id, JVNumber = entity.Jvnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnJournalVoucher
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnJournalVoucher.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }
        #endregion

        #region Combobox
        public async Task<IActionResult> Users([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstUser>();

            using (var context = new FMISContext())
            {
                result = await context.MstUser
                    .OrderBy(x => x.FullName)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Foreign keys
        public async Task<IActionResult> LineBranches() 
        {
            var result = new List<MstBranch>();

            using (var context = new FMISContext())
            {
                result = await context
                       .MstBranch
                       .OrderBy(x => x.Branch)
                       .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> LineAccounts()
        {
            var result = new List<MstAccount>();

            using (var context = new FMISContext())
            {
                result = await context
                       .MstAccount
                       .OrderBy(x => x.Account)
                       .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> LineArticles([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Article)
                    .Where(x => x.ArticleTypeId == 1)
                    .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> LineRRNos([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<TrnReceivingReceipt>();

            using (var context = new FMISContext())
            {
                result = await context.TrnReceivingReceipt
                    .OrderByDescending(x => x.Rrnumber)
                    .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> RRNos()
        {
            var result = new List<TrnReceivingReceipt>();

            using (var context = new FMISContext())
            {
                result = await context.TrnReceivingReceipt
                    .OrderByDescending(x => x.Rrnumber)
                    .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> LineSINos([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<TrnSalesInvoice>();

            using (var context = new FMISContext())
            {
                result = await context.TrnSalesInvoice
                    .OrderByDescending(x => x.Sinumber)
                    .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> SINos()
        {
            var result = new List<TrnSalesInvoice>();

            using (var context = new FMISContext())
            {
                result = await context.TrnSalesInvoice
                    .OrderByDescending(x => x.Sinumber)
                    .ToListAsync();
            }

            return Json(result);
        }

        public IActionResult LineAccountArticles(int accountId) 
        {
            var accountArticles = new Queries.JVArticleQ002();
            accountArticles.AccountParam = accountId;

            var result = accountArticles.Result();

            return Json(result);
        }

        public async Task<IActionResult> AccountArticles()
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderByDescending(x => x.Article)
                    .ToListAsync();
            }

            return Json(result);
        }
        #endregion
    }
}
