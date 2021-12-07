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
    public class TrnStockInController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnStockIn");

            return View();
        }

        #region Grid
        public IActionResult StockInList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnStockIn
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnStockIn>().OrderByDescending(x => x.Innumber);
            }

            return Json(result);
        }

        public IActionResult TrnStockInLineList([DataSourceRequest] DataSourceRequest request, int INId)
        {
            List<TrnStockInItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnStockInItem
                    .Where(x => x.Inid == INId)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult ShowItemComponents([DataSourceRequest] DataSourceRequest request, int ItemId)
        {
            var result = new List<DTO.Costing>();

            using (var context = new FMISContext())
            {
                var query = new Queries.ItemQ002();

                foreach (var component in context.MstArticleComponent.Where(x => x.ArticleId == ItemId)) 
                {
                    result.Add(new DTO.Costing()
                    {
                        Id = component.Id,
                        ArticleId = component.ArticleId,
                        ArticleName = new FMISContext().MstArticle.Find(component.ArticleId).Article,
                        ComponentArticleId = component.ComponentArticleId,
                        ComponentCode = new FMISContext().MstArticle.Find(component.ComponentArticleId).ArticleCode,
                        ComponentName = new FMISContext().MstArticle.Find(component.ComponentArticleId).Article,
                        Quantity = component.Quantity,
                        UnitName = new FMISContext().MstUnit.Find(new FMISContext().MstArticle.Find(component.ComponentArticleId).UnitId).Unit,
                        Cost = query.Result()?.FirstOrDefault(x => x.ArticleId == component.ComponentArticleId)?.FirstOfCost ?? 0,
                        Amount = (query.Result()?.FirstOrDefault(x => x.ArticleId == component.ComponentArticleId)?.FirstOfCost ?? 0) * component.Quantity,
                    });
                }
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.INMaxCode(settings.BranchId);
            var inCode = "";

            if (code != "0000000000" && code != null)
            {
                inCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                inCode = "0000000001";
            }

            var result = new TrnStockIn()
            {
                BranchId = External.ReadSettings().Id,
                Innumber = inCode,
                Indate = DateTime.Now.Date,
                AccountId = TransactionCommon.DefaultInAccountId(),
                ArticleId = TransactionCommon.DefaultSupplierId(settings.BranchId),
                Particulars = "NA",
                ManualInnumber = "NA",
                IsProduced = false,
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> StockInDetail(int Id)
        {
            TrnStockIn result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnStockIn.FindAsync(Id);
            }

            ViewData["INId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddINLine(int inId, int supplierId)
        {
            var branchId = External.ReadSettings().Id;
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.ArticleTypeId == 1);
            var itemInventory = new FMISContext().MstArticleInventory.FirstOrDefault(x => x.ArticleId == item.Id && x.BranchId == External.ReadSettings().Id);

            var result = new TrnStockInItem()
            {
                Id = 0,
                Inid = inId,
                ItemId = item.Id,
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
            var result = new TrnStockIn();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetINById(Id);

                result = context.TrnStockIn
                    .Where(x => x.Innumber.CompareTo(entity.Innumber) < 0)
                    .OrderByDescending(x => x.Innumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnStockIn();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetINById(Id);

                result = context.TrnStockIn
                    .Where(x => x.Innumber.CompareTo(entity.Innumber) > 0)
                    .OrderBy(x => x.Innumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnStockIn data, List<TrnStockInItem> dataSub)
        {
            data.TrnStockInItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnStockIn();
            var entity = new TrnStockIn();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnStockIn.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockInItem.AddRangeAsync(entity.TrnStockInItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnStockInItem.RemoveRange(entity.TrnStockInItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnStockIn>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockIn.AddAsync(entity);
                }

                await context.SaveChangesAsync();

                await Library.Inventory.InsertINInventory(entity);

                await Library.Journal.InsertINJournal(entity);
            }

            return Json(new { Id = entity.Id, INNumber = entity.Innumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnStockIn
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnStockIn.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }

        public IActionResult ShowCost(int ItemId)
        {
            var result = new MstArticle();

            using (var context = new FMISContext()) 
            {
                result = context.MstArticle
                    .Include(x => x.MstArticleComponentArticle)
                    .FirstOrDefault(x => x.Id == ItemId);

                var cost = 0m;

                var query = new Queries.ItemQ002();

                foreach (var item in result.MstArticleComponentArticle) 
                {
                    cost += (query.Result()?.FirstOrDefault(x => x.ArticleId == item.ComponentArticleId)?.FirstOfCost ?? 0) * item.Quantity;
                }

                result.Cost = cost;
            }

            ViewData["ItemId"] = ItemId;

            return PartialView("ShowCost", result);
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
            var articles = new Queries.INLineArticlesQ002();
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

        public IActionResult GetItemCost(int articleId, bool isProduced) 
        {
            var multiplier = 1m;
            var itemCost = 0m;

            var unitId = 0;

            var item = new FMISContext().MstArticle.Find(articleId);

            itemCost = (decimal)item.Cost;
            unitId = item.UnitId;

            if (isProduced)
            {
                using (var context = new FMISContext())
                {
                    var components = context.MstArticleComponent.Where(x => x.ArticleId == articleId)?.ToList();

                    if (components != null && components.Count() > 0)
                    {
                        foreach (var comp in components)
                        {
                            var compCost = context.MstArticleInventory?.FirstOrDefault(x => x.ArticleId == comp.ComponentArticleId && x.BranchId == External.ReadSettings().Id)?.Cost ?? 0;
                            itemCost = itemCost + (comp.Quantity * compCost);
                        }
                    }
                    else
                    {
                        itemCost = context.MstArticleInventory?.FirstOrDefault(x => x.ArticleId == articleId && x.BranchId == External.ReadSettings().Id)?.Cost ?? 0;
                    }

                    multiplier = TransactionCommon.GetMultiplier(articleId, unitId);
                }
            }
            else 
            {
                itemCost = new FMISContext().MstArticleInventory?.FirstOrDefault(x => x.ArticleId == articleId && x.BranchId == External.ReadSettings().Id)?.Cost ?? 0;
            }

            return Json(new { UnitId = unitId, ItemCost = itemCost, Multiplier = multiplier });
        }
        #endregion
    }
}
