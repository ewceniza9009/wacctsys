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
    public class TrnStockTransferController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnStockTransfer");

            return View();
        }

        #region Grid
        public IActionResult StockTransferList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnStockTransfer
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnStockTransfer>().OrderByDescending(x => x.Stnumber);
            }

            return Json(result);
        }

        public IActionResult TrnStockTransferLineList([DataSourceRequest] DataSourceRequest request, int STId)
        {
            List<TrnStockTransferItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnStockTransferItem
                    .Where(x => x.Stid == STId)
                    .ToList();

                foreach (var item in result)
                {
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

            var code = TransactionCommon.STMaxCode(settings.BranchId);
            var stCode = "";

            if (code != "0000000000" && code != null)
            {
                stCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                stCode = "0000000001";
            }

            var result = new TrnStockTransfer()
            {
                BranchId = External.ReadSettings().Id,
                Stnumber = stCode,
                Stdate = DateTime.Now.Date,
                ToBranchId = new FMISContext().MstBranch.FirstOrDefault(x => x.Id != External.ReadSettings().Id).Id,
                Particulars = "NA",
                ManualStnumber = "NA",
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> StockTransferDetail(int Id)
        {
            TrnStockTransfer result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnStockTransfer.FindAsync(Id);
            }

            ViewData["STId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddSTLine(int stId, int supplierId)
        {
            var branchId = External.ReadSettings().Id;
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.ArticleTypeId == 1 && x.IsInventory);
            var itemInventory = new FMISContext().MstArticleInventory?.FirstOrDefault(x => x.ArticleId == item.Id && x.BranchId == External.ReadSettings().Id);

            var result = new TrnStockTransferItem()
            {
                Id = 0,
                Stid = stId,
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
            var result = new TrnStockTransfer();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSTById(Id);

                result = context.TrnStockTransfer
                    .Where(x => x.Stnumber.CompareTo(entity.Stnumber) < 0)
                    .OrderByDescending(x => x.Stnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnStockTransfer();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSTById(Id);

                result = context.TrnStockTransfer
                    .Where(x => x.Stnumber.CompareTo(entity.Stnumber) > 0)
                    .OrderBy(x => x.Stnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnStockTransfer data, List<TrnStockTransferItem> dataSub)
        {
            data.TrnStockTransferItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnStockTransfer();
            var entity = new TrnStockTransfer();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnStockTransfer.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockTransferItem.AddRangeAsync(entity.TrnStockTransferItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnStockTransferItem.RemoveRange(entity.TrnStockTransferItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnStockTransfer>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockTransfer.AddAsync(entity);
                }

                await context.SaveChangesAsync();

                await Library.Inventory.InsertSTInventory(entity);

                await Library.Journal.InsertSTJournal(entity);
            }

            return Json(new { Id = entity.Id, STNumber = entity.Stnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnStockTransfer
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnStockTransfer.Remove(article);

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

        public async Task<IActionResult> Branches([DataSourceRequest] DataSourceRequest request)
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

        public IActionResult LineArticles(int branchId)
        {
            var articles = new Queries.OTLineArticlesQ002();
            articles.Param = branchId;
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
