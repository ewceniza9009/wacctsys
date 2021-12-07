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
    public class TrnPurchaseOrderController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnPurchaseOrder");

            return View();
        }

        #region Grid
        public IActionResult PurchaseOrderList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnPurchaseOrder
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnPurchaseOrder>().OrderByDescending(x => x.Ponumber);
            }

            return Json(result);
        }

        public IActionResult PurchaseOrderLineList([DataSourceRequest] DataSourceRequest request, int POId)
        {
            List<TrnPurchaseOrderItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnPurchaseOrderItem
                    .Where(x => x.Poid == POId)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.POMaxCode(settings.BranchId);
            var poCode = "";

            if (code != "0000000000" && code != null)
            {
                poCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                poCode = "0000000001";
            }

            var result = new TrnPurchaseOrder()
            {
                BranchId = External.ReadSettings().Id,
                Ponumber = poCode,
                Podate = DateTime.Now.Date,
                SupplierId = TransactionCommon.DefaultSupplierId(settings.BranchId),
                TermId = Common.DefaultTermId(),
                ManualPonumber = "NA",
                ManualRequestNumber = "NA",
                DateNeeded = DateTime.Now.Date,
                Remarks = "NA",
                IsClose = false,
                RequestedById = settings.CurrentUserId,
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> PurchaseOrderDetail(int Id)
        {
            TrnPurchaseOrder result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnPurchaseOrder.FindAsync(Id);
            }

            ViewData["POId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddPOLine(int poId)
        {
            var result = new TrnPurchaseOrderItem()
            {
                Id = 0,
                Poid = poId,
                ItemId = TransactionCommon.GetDefaultItem().Id,
                Particulars = "NA",
                UnitId = TransactionCommon.GetDefaultItem().UnitId,
                Quantity = 1,
                Cost = (decimal)TransactionCommon.GetDefaultItem()?.Cost,
                Amount = (decimal)TransactionCommon.GetDefaultItem()?.Cost
            };

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnPurchaseOrder();

            using (var context = new FMISContext())
            {
                var po = TransactionCommon.GetPOById(Id);

                result = context.TrnPurchaseOrder
                    .Where(x => x.Ponumber.CompareTo(po.Ponumber) < 0)
                    .OrderByDescending(x => x.Ponumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnPurchaseOrder();

            using (var context = new FMISContext())
            {
                var po = TransactionCommon.GetPOById(Id);

                result = context.TrnPurchaseOrder
                    .Where(x => x.Ponumber.CompareTo(po.Ponumber) > 0)
                    .OrderBy(x => x.Ponumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TrnPurchaseOrder data, List<TrnPurchaseOrderItem> dataSub)
        {
            data.TrnPurchaseOrderItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();

            var mappingProfile = new MappingProfileForTrnPurchaseOrder();
            var po = new TrnPurchaseOrder();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    po = await context.TrnPurchaseOrder.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, po);

                    TransactionCommon.TransactionDefaults(po);

                    await context.TrnPurchaseOrderItem.AddRangeAsync(po.TrnPurchaseOrderItem.Where(x => x.Id == 0));
                    context.TrnPurchaseOrderItem.RemoveRange(po.TrnPurchaseOrderItem.Where(x => x.IsDeleted));               
                }
                else
                {
                    po = mappingProfile.mapper.Map<TrnPurchaseOrder>(data);

                    TransactionCommon.TransactionDefaults(po);

                    await context.TrnPurchaseOrder.AddAsync(po);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = po.Id, PONumber = po.Ponumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnPurchaseOrder
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnPurchaseOrder.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }
        #endregion

        #region Combobox
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

        public async Task<IActionResult> Terms([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstTerm>();

            using (var context = new FMISContext())
            {
                result = await context.MstTerm
                    .OrderBy(x => x.Term)
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

        public async Task<IActionResult> Articles([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Account)
                    .Where(x => x.ArticleTypeId == 1)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Foreign keys
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
        public async Task<IActionResult> ArticleUnits(int articleId)
        {
            var result = new List<MstUnit>();
            var result2 = new List<MstUnit>();

            using (var context = new FMISContext())
            {
                result = await context
                       .MstUnit
                       .OrderBy(x => x.Unit)
                       .ToListAsync();

                if (articleId != 0)
                {
                    foreach (var item in result) 
                    {
                        item.Articles = context.MstArticleUnit
                            .Where(x => x.UnitId == item.Id)
                            .Select(x => x.ArticleId)
                            .ToList();
                    }

                    result2 = result
                        .Where(x => x.Articles.Any(y => y == articleId))
                        .ToList();

                    return Json(result2);
                }               
            }

            return Json(result);
        }
        #endregion
    }
}
