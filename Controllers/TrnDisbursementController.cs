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
    public class TrnDisbursementController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnDisbursement");

            return View();
        }

        #region Grid
        public IActionResult DisbursementList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnDisbursement
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnDisbursement>().OrderByDescending(x => x.Cvnumber);
            }

            return Json(result);
        }

        public IActionResult TrnDisbursementLineList([DataSourceRequest] DataSourceRequest request, int CVId)
        {
            List<TrnDisbursementLine> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnDisbursementLine
                    .Where(x => x.Cvid == CVId)
                    .ToList();               
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult SupplierAdvances([DataSourceRequest] DataSourceRequest request) 
        {
            var result = new List<DTO.SupplierAdvance>();

            using (var context = new FMISContext())
            {
                var query = new Queries.SupplierAdvanceQ002();

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
                                    query.ParamSupplierId = int.Parse(descriptor.Value.ToString());

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

                    query.ParamSupplierAdvancesAccountId = 186;

                    result = query.Result();
                }
            }

            if (result.Count == 0)
            {
                return Json(result);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult RRStatusLines([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<DTO.TmpReceivingReceiptStatus>();

            using (var context = new FMISContext())
            {
                var query = new Queries.CVRRStatusQ002();

                if (request.Filters.Count > 0)
                {
                    foreach (var item in request.Filters.Cast<Kendo.Mvc.CompositeFilterDescriptor>())
                    {
                        foreach (var descriptor in item.FilterDescriptors.Cast<Kendo.Mvc.FilterDescriptor>()) 
                        {
                            if (descriptor.Member == "SupplierId")
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
                    query.ReadRRStatus();
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

            var code = TransactionCommon.CVMaxCode(settings.BranchId);
            var cvCode = "";

            if (code != "0000000000" && code != null)
            {
                cvCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                cvCode = "0000000001";
            }

            var defaultSupplierId = TransactionCommon.DefaultSupplierId(settings.BranchId);
            var defaultPayee = new FMISContext().MstArticle?.Find(defaultSupplierId)?.ContactPerson ?? "NA";

            var result = new TrnDisbursement()
            {
                BranchId = External.ReadSettings().Id,
                Cvnumber = cvCode,
                Cvdate = DateTime.Now.Date,
                SupplierId = defaultSupplierId,
                Payee = defaultPayee,
                PayTypeId = TransactionCommon.DefaultPayTypeId(),
                BankId = TransactionCommon.DefaultBankId(),
                ManualCvnumber = "NA",
                CheckNumber = "NA",
                CheckDate = DateTime.Now.Date,
                Particulars = "NA",
                IsCrossCheck = false,
                IsClear = false,
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> DisbursementDetail(int Id)
        {
            TrnDisbursement result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnDisbursement.FindAsync(Id);
            }

            ViewData["CVId"] = Id;

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> DownloadPayables(int Id, int supplierId)
        {
            TrnDisbursement result = null;

            if (Id == 0)
            {
                result = new TrnDisbursement()
                {
                    SupplierId = supplierId
                };
            }
            else
            {
                using (var context = new FMISContext())
                {
                    result = await context.TrnDisbursement.FindAsync(Id);
                }
            }

            return PartialView("DownloadPayables", result);
        }

        public async Task<IActionResult> DownloadAdvances(int Id, int supplierId)
        {
            TrnDisbursement result = null;

            if (Id == 0)
            {
                result = new TrnDisbursement()
                {
                    SupplierId = supplierId
                };
            }
            else
            {
                using (var context = new FMISContext())
                {
                    result = await context.TrnDisbursement.FindAsync(Id);
                }
            }

            return PartialView("DownloadSupplierAdvances", result);
        }

        public IActionResult AddCVLine(int cvId)
        {
            var result = new TrnDisbursementLine()
            {
                Id = 0,
                Cvid = cvId,
                BranchId = External.ReadSettings().Id,
                AccountId = Common.GetAccountById(224).Id,
                ArticleId = TransactionCommon.DefaultSupplierId(External.ReadSettings().Id),
                Particulars = "NA",
                Rrid = TransactionCommon.DefaultRRId(External.ReadSettings().Id),
                Amount = 0
            };

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnDisbursement();

            using (var context = new FMISContext())
            {
                var cv = TransactionCommon.GetCVById(Id);

                result = context.TrnDisbursement
                    .Where(x => x.Cvnumber.CompareTo(cv.Cvnumber) < 0)
                    .OrderByDescending(x => x.Cvnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnDisbursement();

            using (var context = new FMISContext())
            {
                var cv = TransactionCommon.GetCVById(Id);

                result = context.TrnDisbursement
                    .Where(x => x.Cvnumber.CompareTo(cv.Cvnumber) > 0)
                    .OrderBy(x => x.Cvnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TrnDisbursement data, List<TrnDisbursementLine> dataSub)
        {
            data.TrnDisbursementLine = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();

            var mappingProfile = new MappingProfileForTrnDisbursement();
            var entity = new TrnDisbursement();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnDisbursement.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnDisbursementLine.AddRangeAsync(entity.TrnDisbursementLine.Where(x => x.Id == 0));
                    context.TrnDisbursementLine.RemoveRange(entity.TrnDisbursementLine.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnDisbursement>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnDisbursement.AddAsync(entity);
                }

                await context.SaveChangesAsync();

                await Library.Journal.InsertCVJournal(entity);

                foreach (var item in entity.TrnDisbursementLine.Where(x => x.Rrid != null))
                {
                    Library.Balances.UpdateAP((int)item.Rrid);
                }
            }

            return Json(new { Id = entity.Id, CVNumber = entity.Cvnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnDisbursement
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnDisbursement.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }
        #endregion

        #region ComboBox
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

        public async Task<IActionResult> Suppliers([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Account)
                    .Where(x => x.ArticleTypeId == 3)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> PayTypes([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstPayType>();

            using (var context = new FMISContext())
            {
                result = await context.MstPayType
                    .OrderBy(x => x.Account)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Banks([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Account)
                    .Where(x => x.ArticleTypeId == 5)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Foreign Keys
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

        public IActionResult LineAccountArticles(int accountId)
        {
            var accountArticles = new Queries.JVArticleQ002();
            accountArticles.AccountParam = accountId;

            var result = accountArticles.Result();

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

        [HttpPost]
        public IActionResult DownloadRRLinesSelected(int cvId, List<DTO.TmpReceivingReceiptStatus> rrLinesSelected)
        {
            var result = new List<TrnDisbursementLine>();

            foreach (var line in rrLinesSelected.Where(x => (x?.Pick ?? false) && x.Amount > 0))
            {
                var article = new FMISContext().MstArticle.Find(line.SupplierId);

                result.Add(new TrnDisbursementLine()
                {
                    Id = 0,
                    Cvid = cvId,
                    Rrid = line.RRId,
                    Particulars = line.Particulars,
                    BranchId = line.BranchId,
                    AccountId = article.AccountId,
                    ArticleId = line.SupplierId,
                    Amount = line.Amount
                });
            }

            return Json(new { Result = result });
        }
        
        public IActionResult DownloadAdvLinesSelected(int cvId, List<DTO.SupplierAdvance> supplierAdvanceSelected)
        {
            var result = new List<TrnDisbursementLine>();

            foreach (var line in supplierAdvanceSelected)
            {
                result.Add(new TrnDisbursementLine()
                {
                    Id = 0,
                    Cvid = cvId,
                    Particulars = "Supplier Advance",
                    BranchId = line.BranchId,
                    AccountId = line.AccountId,
                    ArticleId = line.ArticleId,
                    Amount = line.Balance
                });
            }

            return Json(new { Result = result });
        }
        #endregion
    }
}
