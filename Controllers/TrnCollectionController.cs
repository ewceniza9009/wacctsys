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
    public class TrnCollectionController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnCollection");

            return View();
        }

        #region Grid
        public IActionResult CollectionList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnCollection
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnCollection>().OrderByDescending(x => x.Ornumber);
            }

            return Json(result);
        }

        public IActionResult TrnCollectionLineList([DataSourceRequest] DataSourceRequest request, int ORId)
        {
            List<TrnCollectionLine> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnCollectionLine
                    .Where(x => x.Orid == ORId)
                    .ToList();

                var headerBranchId = context.TrnCollection.Find(ORId)?.BranchId ?? External.ReadSettings().Id;

                foreach (var item in result)
                {
                    item.BranchName = new FMISContext().MstBranch.Find(item.BranchId).Branch;
                    item.AccountName = new FMISContext().MstAccount.Find(item.AccountId).Account;
                    item.ArticleName = new FMISContext().MstArticle.Find(item.ArticleId).Article;
                    item.SINumber = new FMISContext().TrnSalesInvoice.Find(item.Siid)?.Sinumber;
                    item.PayTypeName = new FMISContext().MstPayType.Find(item.PayTypeId).PayType;
                }
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult CustomerAdvances([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<DTO.CustomerAdvance>();

            using (var context = new FMISContext())
            {
                var query = new Queries.CustomerAdvanceQ002();

                if (request.Filters.Count > 0)
                {
                    var filter = request.Filters[0] as Kendo.Mvc.FilterDescriptor;

                    foreach (var item in request.Filters.Cast<Kendo.Mvc.CompositeFilterDescriptor>())
                    {
                        foreach (var descriptor in item.FilterDescriptors.Cast<Kendo.Mvc.FilterDescriptor>())
                        {
                            if (descriptor.Member == "ArticleId")
                            {
                                if (!string.IsNullOrEmpty(descriptor.Value?.ToString()))
                                {
                                    query.ParamCustomerId = int.Parse(descriptor.Value.ToString());

                                    continue;
                                }
                            }

                            if (descriptor.Member == "BranchId")
                            {
                                if (!string.IsNullOrEmpty(descriptor.Value?.ToString()))
                                {
                                    query.ParamBranchId = int.Parse(descriptor.Value.ToString());

                                    continue;
                                }
                            }
                        }

                        break;
                    }

                    query.ParamCustomerAdvancesAccountId = 325;

                    result = query.Result();
                }
            }

            if (result.Count == 0)
            {
                return Json(result);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult SIStatusLines([DataSourceRequest] DataSourceRequest request)
        {
            List<DTO.TmpSalesInvoiceStatus> result = new List<DTO.TmpSalesInvoiceStatus>();

            using (var context = new FMISContext())
            {
                var query = new Queries.ORSIStatusQ002();

                if (request.Filters.Count > 0)
                {
                    foreach (var item in request.Filters.Cast<Kendo.Mvc.CompositeFilterDescriptor>())
                    {
                        foreach (var descriptor in item.FilterDescriptors.Cast<Kendo.Mvc.FilterDescriptor>())
                        {
                            if (descriptor.Member == "CustomerId")
                            {
                                if (!string.IsNullOrEmpty(descriptor.Value.ToString()))
                                {
                                    query.ParamId = int.Parse(descriptor.Value.ToString());

                                    continue;
                                }
                            }

                            if (descriptor.Member == "BranchId")
                            {
                                if (!string.IsNullOrEmpty(descriptor.Value.ToString()))
                                {
                                    query.ParamBranchId = int.Parse(descriptor.Value.ToString());

                                    continue;
                                }
                            }
                        }

                        break;
                    }
                }

                if (query.ParamId != 0 && query.ParamBranchId != 0)
                {
                    query.ReadSIStatus();
                    result = query.Result();
                }
            }

            if (result.Count == 0)
            {
                return Json(result);
            }

            var finalResult = new DataSourceResult();

            finalResult.AggregateResults = null;
            finalResult.Errors = null;
            finalResult.Data = result;
            finalResult.Total = result.Count;

            return Json(finalResult);
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.ORMaxCode(settings.BranchId);
            var orCode = "";

            if (code != "0000000000" && code != null)
            {
                orCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                orCode = "0000000001";
            }

            var result = new TrnCollection()
            {
                BranchId = External.ReadSettings().Id,
                Ornumber = orCode,
                Ordate = DateTime.Now.Date,
                CustomerId = TransactionCommon.DefaultCustomerId(settings.BranchId),
                Particulars = "NA",
                ManualOrnumber = "NA",
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> CollectionDetail(int Id)
        {
            TrnCollection result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnCollection.FindAsync(Id);
            }

            ViewData["ORId"] = Id;

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> DownloadReceivables(int Id, int customerId)
        {
            TrnCollection result = null;

            if (Id == 0)
            {
                result = new TrnCollection()
                {
                    CustomerId = customerId
                };
            }
            else
            {
                using (var context = new FMISContext())
                {
                    result = await context.TrnCollection.FindAsync(Id);
                }
            }

            return PartialView("DownloadReceivables", result);
        }

        public async Task<IActionResult> DownloadAdvances(int Id, int customerId)
        {
            TrnCollection result = null;

            if (Id == 0)
            {
                result = new TrnCollection()
                {
                    CustomerId = customerId
                };
            }
            else
            {
                using (var context = new FMISContext())
                {
                    result = await context.TrnCollection.FindAsync(Id);
                }
            }

            return PartialView("DownloadCustomerAdvances", result);
        }

        public IActionResult AddORLine(int orId, int customerId)
        {
            var branchId = External.ReadSettings().Id;

            var result = new TrnCollectionLine()
            {
                Id = 0,
                Orid = orId,
                Siid = TransactionCommon.GetMaxSIId(branchId),
                BranchId = branchId,
                AccountId = Common.ArticleAccountId(),
                ArticleId = TransactionCommon.DefaultCustomerId(branchId),
                Particulars = "NA",
                Amount = 0,
                PayTypeId = TransactionCommon.DefaultPayTypeId(),
                CheckNumber = "NA",
                CheckDate = DateTime.Now.Date,
                CheckBank = "NA",
                DepositoryBankId = TransactionCommon.DefaultDepositoryBank().Id,
                IsClear = false,
                IsDeleted = false,
                BranchName = External.ReadSettings().Branch,
                AccountName = new FMISContext().MstAccount.Find(Common.ArticleAccountId()).Account,
                ArticleName = new FMISContext().MstArticle.Find(TransactionCommon.DefaultCustomerId(branchId)).Article,
                PayTypeName = new FMISContext().MstPayType.Find(TransactionCommon.DefaultPayTypeId()).PayType
            };

            ViewData["CustomerId"] = customerId;

            return PartialView("LineDetail", result);
        }

        [HttpPost]
        public IActionResult DetailORLine(TrnCollectionLine orLine)
        {
            orLine.BranchName = new FMISContext().MstBranch.Find(orLine.BranchId).Branch;
            orLine.AccountName = new FMISContext().MstAccount.Find(orLine.AccountId).Account;
            orLine.ArticleName = new FMISContext().MstArticle.Find(orLine.ArticleId).Article;
            orLine.SINumber = new FMISContext().TrnSalesInvoice.Find(orLine.Siid)?.Sinumber;
            orLine.PayTypeName = new FMISContext().MstPayType.Find(orLine.PayTypeId).PayType;

            return PartialView("LineDetail", orLine);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnCollection();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetORById(Id);

                result = context.TrnCollection
                    .Where(x => x.Ornumber.CompareTo(entity.Ornumber) < 0)
                    .OrderByDescending(x => x.Ornumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnCollection();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetORById(Id);

                result = context.TrnCollection
                    .Where(x => x.Ornumber.CompareTo(entity.Ornumber) > 0)
                    .OrderBy(x => x.Ornumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnCollection data, List<TrnCollectionLine> dataSub)
        {
            data.TrnCollectionLine = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnCollection();
            var entity = new TrnCollection();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnCollection.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnCollectionLine.AddRangeAsync(entity.TrnCollectionLine.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnCollectionLine.RemoveRange(entity.TrnCollectionLine.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnCollection>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnCollection.AddAsync(entity);
                }

                await context.SaveChangesAsync();

                await Library.Journal.InsertORJournal(entity);

                foreach (var item in entity.TrnCollectionLine.Where(x => x.Siid != null))
                {
                    Library.Balances.UpdateAR((int)item.Siid);
                }
            }

            return Json(new { Id = entity.Id, ORNumber = entity.Ornumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnCollection
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnCollection.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }
        #endregion

        #region Combobox
        public async Task<IActionResult> Customers([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Account)
                    .Where(x => x.ArticleTypeId == 2)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }


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

        public async Task<IActionResult> LineBranches([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstBranch>();

            using (var context = new FMISContext())
            {
                result = await context.MstBranch
                    .OrderBy(x => x.Branch)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> LineAccounts([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstAccount>();

            using (var context = new FMISContext())
            {
                result = await context.MstAccount
                    .OrderBy(x => x.Account)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineArticles([DataSourceRequest] DataSourceRequest request, int accountId)
        {
            var articles = new Queries.ORArticlesQ002();
            articles.AccountParam = accountId;

            var result = articles.Result();

            return Json(result.ToDataSourceResult(request));
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

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> LinePayTypes([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstPayType>();

            using (var context = new FMISContext())
            {
                result = await context.MstPayType
                    .OrderBy(x => x.PayType)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> LineBanks([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .Where(x => x.ArticleTypeId == 5)
                    .OrderBy(x => x.Article)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        #endregion

        #region Foreign keys

        #endregion

        #region APIS
        public IActionResult GetORLineInfos(int branchId, int accountId, int articleId, int siid, int payTypeId)
        {
            var result = new string[5];

            using (var context = new FMISContext())
            {
                result[0] = context.MstBranch.Find(branchId).Branch;
                result[1] = context.MstAccount.Find(accountId).Account;
                result[2] = context.MstArticle.Find(articleId).Article;
                result[3] = context.TrnSalesInvoice.Find(siid).Sinumber;
                result[4] = context.MstPayType.Find(payTypeId).PayType;
            }

            return Json(new { Branch = result[0], Account = result[1], Article = result[2], SINo = result[3], PayType = result[4] });
        }

        [HttpPost]
        public IActionResult DownloadSILinesSelected(int orId, List<DTO.TmpSalesInvoiceStatus> siLinesSelected)
        {
            var result = new List<TrnCollectionLine>();

            foreach (var line in siLinesSelected.Where(x => (x?.Pick ?? false) && x.Amount > 0))
            {
                var article = new FMISContext().MstArticle.Find(line.CustomerId);

                result.Add(new TrnCollectionLine()
                {
                    Id = 0,
                    Orid = orId,
                    Siid = line.SIId,
                    SINumber = new FMISContext().TrnSalesInvoice.Find(line.SIId).Sinumber,
                    Particulars = line.Particulars,
                    BranchId = line.BranchId,
                    BranchName = new FMISContext().MstBranch.Find(line.BranchId).Branch,
                    AccountId = article.AccountId,
                    AccountName = new FMISContext().MstAccount.Find(article.AccountId).Account,
                    ArticleId = line.CustomerId,
                    ArticleName = new FMISContext().MstArticle.Find(line.CustomerId).Article,
                    Amount = line.Amount,
                    PayTypeId = TransactionCommon.DefaultPayTypeId(),
                    PayTypeName = new FMISContext().MstPayType.Find(TransactionCommon.DefaultPayTypeId()).PayType,
                    CheckNumber = "NA",
                    CheckDate = DateTime.Now.Date,
                    CheckBank = "NA",
                    DepositoryBankId = TransactionCommon.DefaultDepositoryBank().Id,
                    IsClear = true
                });
            }

            return Json(new { Result = result });
        }

        public IActionResult DownloadAdvLinesSelected(int orId, List<DTO.CustomerAdvance> customerAdvanceSelected)
        {
            var result = new List<TrnCollectionLine>();

            foreach (var line in customerAdvanceSelected)
            {
                var article = new FMISContext().MstArticle.Find(line.ArticleId);

                result.Add(new TrnCollectionLine()
                {
                    Id = 0,
                    Orid = orId,
                    Particulars = "Customer Advance",
                    BranchId = line.BranchId,
                    BranchName = new FMISContext().MstBranch.Find(line.BranchId).Branch,
                    AccountId = article.AccountId,
                    AccountName = new FMISContext().MstAccount.Find(article.AccountId).Account,
                    ArticleId = line.ArticleId,
                    ArticleName = new FMISContext().MstArticle.Find(line.ArticleId).Article,
                    Amount = Math.Abs(line.Balance),
                    PayTypeId = 9, //TransactionCommon.DefaultPayTypeId(),
                    PayTypeName = new FMISContext().MstPayType.Find(9).PayType, //new FMISContext().MstPayType.Find(TransactionCommon.DefaultPayTypeId()).PayType,
                    CheckNumber = "NA",
                    CheckDate = DateTime.Now.Date,
                    CheckBank = "NA",
                    IsClear = true
                });
            }

            return Json(new { Result = result });
        }
        #endregion
    }
}
