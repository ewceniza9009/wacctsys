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
    public class TrnStockOutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnStockOut");
            return View();
        }

        #region Grid
        public IActionResult StockOutList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnStockOut
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnStockOut>().OrderByDescending(x => x.Otnumber);
            }

            return Json(result);
        }

        public IActionResult TrnStockOutLineList([DataSourceRequest] DataSourceRequest request, int OTId)
        {
            List<TrnStockOutItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnStockOutItem
                    .Where(x => x.Otid == OTId)
                    .ToList();

                foreach (var item in result) 
                {
                    item.ExpenseAccountName = context.MstAccount.Find(item.ExpenseAccountId).Account;
                    item.InventoryCode = context.MstArticleInventory.Find(item.ItemInventoryId).InventoryCode;
                }
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.OTMaxCode(settings.BranchId);
            var otCode = "";

            if (code != "0000000000" && code != null)
            {
                otCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                otCode = "0000000001";
            }

            var result = new TrnStockOut()
            {
                BranchId = External.ReadSettings().Id,
                Otnumber = otCode,
                Otdate = DateTime.Now.Date,
                AccountId = TransactionCommon.DefaultInAccountId(),
                ArticleId = TransactionCommon.DefaultSupplierId(settings.BranchId),
                Particulars = "NA",
                ManualOtnumber = "NA",
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> StockOutDetail(int Id)
        {
            TrnStockOut result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnStockOut.FindAsync(Id);
            }

            ViewData["OTId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddOTLine(int otId, int supplierId)
        {
            var branchId = External.ReadSettings().Id;
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.ArticleTypeId == 1 && x.IsInventory);
            var itemInventory = new FMISContext().MstArticleInventory?.FirstOrDefault(x => x.ArticleId == item.Id && x.BranchId == External.ReadSettings().Id);

            var result = new TrnStockOutItem()
            {
                Id = 0,
                Otid = otId,
                ExpenseAccountId = item.ExpenseAccountId,
                ExpenseAccountName = new FMISContext().MstAccount.Find(item.ExpenseAccountId).Account,
                ItemId = item.Id,
                ItemInventoryId = itemInventory.Id,
                InventoryCode = itemInventory.InventoryCode,
                Particulars = "NA",
                Quantity = 1,
                UnitId = item.UnitId,
                Cost = itemInventory?.Cost ?? 0,
                Amount = itemInventory?.Cost ?? 0,
                BaseUnitId = item.UnitId,
                BaseQuantity = 1,
                BaseCost = itemInventory?.Cost ?? 0
            };

            ViewData["SupplierId"] = supplierId;

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnStockOut();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetOTById(Id);

                result = context.TrnStockOut
                    .Where(x => x.Otnumber.CompareTo(entity.Otnumber) < 0)
                    .OrderByDescending(x => x.Otnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnStockOut();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetOTById(Id);

                result = context.TrnStockOut
                    .Where(x => x.Otnumber.CompareTo(entity.Otnumber) > 0)
                    .OrderBy(x => x.Otnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnStockOut data, List<TrnStockOutItem> dataSub)
        {
            data.TrnStockOutItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnStockOut();
            var entity = new TrnStockOut();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnStockOut.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockOutItem.AddRangeAsync(entity.TrnStockOutItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnStockOutItem.RemoveRange(entity.TrnStockOutItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnStockOut>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockOut.AddAsync(entity);
                }

                await context.SaveChangesAsync();

                await Library.Inventory.InsertOTInventory(entity);

                await Library.Journal.InsertOTJournal(entity);
            }

            return Json(new { Id = entity.Id, OTNumber = entity.Otnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnStockOut
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnStockOut.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }
        #endregion

        #region Combobox
        public async Task<IActionResult> Accounts([DataSourceRequest] DataSourceRequest request)
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

        public IActionResult Articles([DataSourceRequest] DataSourceRequest request)
        {
            var articles = new Queries.INArticleQ002();
            var result = articles.Result();

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

        public IActionResult LineUnits([DataSourceRequest] DataSourceRequest request, int articleId)
        {
            var units = new Queries.RRUnitQ002();
            units.ArticleParam = articleId;

            var result = units.Result();

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineBaseUnits()
        {
            var result = new List<MstUnit>();

            using (var context = new FMISContext())
            {
                result = context.MstUnit.ToList();
            }

            return Json(result);
        }
        #endregion

        #region Foreign keys

        public IActionResult LineArticles()
        {
            var articles = new Queries.OTLineArticlesQ002();
            articles.Param = External.ReadSettings().Id;
            var result = articles.Result();

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

        public IActionResult LineAccounts()
        {
            var result = new List<MstAccount>();

            using (var context = new FMISContext()) 
            {
                result = context.MstAccount.OrderBy(x => x.Account).ToList();
            }

            return Json(result);
        }
        #endregion

        #region APIS
        public IActionResult ComputeBaseUnit(int itemId, int unitId, decimal quantity, decimal amount)
        {
            var resultBaseQuantity = 0m;
            var resultBaseCost = 0m;

            var multiplier = TransactionCommon.GetMultiplier(itemId, unitId);

            var resultBaseUnitId = new FMISContext().MstArticle.Find(itemId).UnitId;

            resultBaseQuantity = quantity / multiplier;

            if (quantity == 0)
            {
                resultBaseCost = 0;
            }
            else
            {
                if (quantity > 0)
                {
                    resultBaseCost = Math.Round(amount / (quantity / multiplier), 4);
                }
                else
                {
                    resultBaseCost = 0;
                }
            }

            return Json(new { BaseQuantity = resultBaseQuantity, BaseCost = resultBaseCost, BaseUnitId = resultBaseUnitId });
        }

        public IActionResult GetItemCost(int articleId, int branchId)
        {
            var itemCost = 0m;
            var itemInventoryCode = "";

            using (var context = new FMISContext())
            {
                var itemInventory = context.MstArticleInventory.FirstOrDefault(x => x.ArticleId == articleId && x.BranchId == branchId);
                itemCost = itemInventory.Cost;
                itemInventoryCode = itemInventory.InventoryCode;
            }

            return Json(new { ItemCost = itemCost, ItemInventoryCode = itemInventoryCode });
        }

        public IActionResult ComputeCost(int itemId, int unitId, decimal cost) 
        {
            var baseUnitId = new FMISContext().MstArticle.Find(itemId).UnitId;
            var multiplier = TransactionCommon.GetMultiplier(itemId, unitId);
            var cost1 = cost / multiplier;

            return Json(new { BaseUnitId = baseUnitId, Cost = cost1 });
        }
        #endregion
    }
}
