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
    public class TrnStockCountController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnStockCount");

            return View();
        }

        #region Grid
        public IActionResult StockCountList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnStockCount
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnStockCount>().OrderByDescending(x => x.Scnumber);
            }

            return Json(result);
        }

        public IActionResult TrnStockCountLineList([DataSourceRequest] DataSourceRequest request, int SCId)
        {
            List<TrnStockCountItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnStockCountItem
                    .Where(x => x.Scid == SCId)
                    .ToList();

                foreach (var item in result) 
                {
                    var article = new FMISContext().MstArticle.Find(item.ItemId);
                    item.UnitName = new FMISContext().MstUnit.Find(article.UnitId).Unit;
                }
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.SCMaxCode(settings.BranchId);
            var scCode = "";

            if (code != "0000000000" && code != null)
            {
                scCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                scCode = "0000000001";
            }

            var result = new TrnStockCount()
            {
                BranchId = External.ReadSettings().Id,
                Scnumber = scCode,
                Scdate = DateTime.Now.Date,
                Particulars = "NA",
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> StockCountDetail(int Id)
        {
            TrnStockCount result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnStockCount.FindAsync(Id);
            }

            ViewData["SCId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddSCLine(int scId, int supplierId)
        {
            var branchId = External.ReadSettings().Id;
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.ArticleTypeId == 1 && x.IsInventory);

            var result = new TrnStockCountItem()
            {
                Id = 0,
                Scid = scId,
                ItemId = item.Id,
                Particulars = "NA",
                Quantity = 1,
                UnitName = new FMISContext().MstUnit.Find(item.UnitId).Unit
            };

            ViewData["SupplierId"] = supplierId;

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnStockCount();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSCById(Id);

                result = context.TrnStockCount
                    .Where(x => x.Scnumber.CompareTo(entity.Scnumber) < 0)
                    .OrderByDescending(x => x.Scnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnStockCount();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSCById(Id);

                result = context.TrnStockCount
                    .Where(x => x.Scnumber.CompareTo(entity.Scnumber) > 0)
                    .OrderBy(x => x.Scnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnStockCount data, List<TrnStockCountItem> dataSub)
        {
            data.TrnStockCountItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnStockCount();
            var entity = new TrnStockCount();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnStockCount.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockCountItem.AddRangeAsync(entity.TrnStockCountItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnStockCountItem.RemoveRange(entity.TrnStockCountItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnStockCount>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnStockCount.AddAsync(entity);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = entity.Id, SCNumber = entity.Scnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnStockCount
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnStockCount.Remove(article);

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
        public async Task<IActionResult> LineArticles()
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .Where(x => x.ArticleTypeId == 1)
                    .OrderBy(x => x.Article)
                    .ToListAsync();
            }

            return Json(result);
        }
        #endregion

        #region APIS
        public IActionResult GetUnitName(int itemId)
        {
            var item = new FMISContext().MstArticle.Find(itemId);
            var result = new FMISContext().MstUnit.Find(item.UnitId);

            return Json(new { UnitName = result.Unit });
        }
        #endregion
    }
}
