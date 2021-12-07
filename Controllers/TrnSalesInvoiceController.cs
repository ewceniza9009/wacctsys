using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class TrnSalesInvoiceController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnSalesInvoice");

            return View();
        }

        #region Grid
        public IActionResult SalesInvoiceList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnSalesInvoice
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnSalesInvoice>().OrderByDescending(x => x.Sinumber);
            }

            return Json(result);
        }

        public IActionResult TrnSalesInvoiceLineList([DataSourceRequest] DataSourceRequest request, int SIId)
        {
            List<TrnSalesInvoiceItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnSalesInvoiceItem
                    .Where(x => x.Siid == SIId)
                    .ToList();

                var headerBranchId = context.TrnSalesInvoice.Find(SIId)?.BranchId ?? External.ReadSettings().Id;

                foreach (var item in result)
                {
                    item.ItemInventoryCode = new FMISContext().MstArticleInventory.FirstOrDefault(x => x.ArticleId == item.ItemId && x.BranchId == headerBranchId)?.InventoryCode ?? "NA";
                    item.ItemName = new FMISContext().MstArticle.Find(item.ItemId).Article;
                    item.UnitName = new FMISContext().MstUnit.Find(item.UnitId).Unit;
                    item.VatInfo = $"@{Math.Round(item.Vatpercentage, 2)}% {new FMISContext().MstTaxType.Find(item.Vatid).TaxType} => {Math.Round(item.Vatamount, 2)}";
                    item.UnitConversionInfo = $"{Math.Round(item.BaseQuantity, 2)} {new FMISContext().MstUnit.Find(item.BaseUnitId).Unit} Price: {Math.Round(item.BasePrice, 2)}";
                }
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.SIMaxCode(settings.BranchId);
            var siCode = "";

            if (code != "0000000000" && code != null)
            {
                siCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                siCode = "0000000001";
            }

            var result = new TrnSalesInvoice()
            {
                BranchId = External.ReadSettings().Id,
                Sinumber = siCode,
                Sidate = DateTime.Now.Date,
                CustomerId = TransactionCommon.DefaultCustomerId(settings.BranchId),
                TermId = Common.DefaultTermId(),
                DocumentReference = "NA",
                Remarks = "NA",
                ManualSinumber = "NA",
                SoldById = settings.CurrentUserId,
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> SalesInvoiceDetail(int Id)
        {
            TrnSalesInvoice result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnSalesInvoice.FindAsync(Id);
            }

            ViewData["SIId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddSILine(int siId, int customerId)
        {
            var branchId = External.ReadSettings().Id;
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.ArticleTypeId == 1);
            var inventoryItem = new FMISContext().MstArticleInventory?.FirstOrDefault(x => x.ArticleId == item.Id && x.BranchId == branchId);
            var inventoryUnit = new FMISContext().MstArticleUnit?.FirstOrDefault(x => x.ArticleId == item.Id && x.UnitId == item.UnitId);

            var itemArticleImage = item?.ArticleImage;
            var itemArticleImagePath = "";

            if (string.IsNullOrEmpty(itemArticleImage))
            {
                itemArticleImagePath = $@"/itemImages/no-image.png";
            }
            else
            {
                itemArticleImagePath = $@"/itemImages/{itemArticleImage}";
            }

            var result = new TrnSalesInvoiceItem()
            {
                Id = 0,
                Siid = siId,
                ItemId = item.Id,
                ItemInventoryId = (inventoryItem?.Id ?? 0),
                Particulars = "NA",
                Quantity = 1,
                UnitId = item.UnitId,
                Price = item.Price,
                DiscountId = TransactionCommon.DefaultDiscount().Id,
                DiscountRate = TransactionCommon.DefaultDiscount().DiscountRate,
                DiscountAmount = TransactionCommon.DefaultDiscount().DiscountRate,
                NetPrice = item.Price,
                Amount = item.Price,
                Vatid = Common.OutputTaxId(),
                Vatpercentage = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id == Common.InputTaxId())?.TaxRate ?? 0,
                Vatamount = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id == Common.InputTaxId())?.TaxRate ?? 0,
                BaseUnitId = item.UnitId,
                BaseQuantity = 1,
                BasePrice = Math.Round(item.Price / (inventoryUnit?.Multiplier ?? 0) , 4),
                VatInfo = "Select Item",
                WTaxInfo = "Select Item",
                UnitConversionInfo = "Select Item",
                ArticleImagePath = itemArticleImagePath
            };

            ViewData["CustomerId"] = customerId;

            return PartialView("LineDetail", result);
        }

        [HttpPost]
        public IActionResult DetailSILine(TrnSalesInvoiceItem siLine)
        {
            siLine.ItemName = new FMISContext().MstArticle.Find(siLine.ItemId).Article;
            siLine.UnitName = new FMISContext().MstUnit.Find(siLine.UnitId).Unit;
            siLine.VatInfo = $"@{Math.Round(siLine.Vatpercentage, 2)}% {new FMISContext().MstTaxType.Find(siLine.Vatid).TaxType} => {Math.Round(siLine.Vatamount, 2)}";
            siLine.UnitConversionInfo = $"{Math.Round(siLine.BaseQuantity, 2)} {new FMISContext().MstUnit.Find(siLine.BaseUnitId).Unit} Price: {Math.Round(siLine.BasePrice, 2)}";

            var itemArticleImage = new FMISContext().MstArticle.Find(siLine.ItemId)?.ArticleImage;

            if (string.IsNullOrEmpty(itemArticleImage))
            {
                siLine.ArticleImagePath = $@"/itemImages/no-image.png";
            }
            else
            {
                siLine.ArticleImagePath = $@"/itemImages/{itemArticleImage}";
            }

            return PartialView("LineDetail", siLine);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnSalesInvoice();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSIById(Id);

                result = context.TrnSalesInvoice
                    .Where(x => x.Sinumber.CompareTo(entity.Sinumber) < 0)
                    .OrderByDescending(x => x.Sinumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnSalesInvoice();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetSIById(Id);

                result = context.TrnSalesInvoice
                    .Where(x => x.Sinumber.CompareTo(entity.Sinumber) > 0)
                    .OrderBy(x => x.Sinumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnSalesInvoice data, List<TrnSalesInvoiceItem> dataSub)
        {
            data.TrnSalesInvoiceItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnSalesInvoice();
            var entity = new TrnSalesInvoice();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnSalesInvoice.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnSalesInvoiceItem.AddRangeAsync(entity.TrnSalesInvoiceItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnSalesInvoiceItem.RemoveRange(entity.TrnSalesInvoiceItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnSalesInvoice>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnSalesInvoice.AddAsync(entity);
                }

                entity.Amount = entity.TrnSalesInvoiceItem.Sum(x => x.Amount);

                await context.SaveChangesAsync();

                await Library.Inventory.InsertSIInventory(entity);

                await Library.Journal.InsertSIJournal(entity);

                Library.Balances.UpdateAR(entity.Id);
            }

            return Json(new { Id = entity.Id, SINumber = entity.Sinumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnSalesInvoice
                    .FirstOrDefault(x => x.Id == Id);

                var articleInventories = article.TrnSalesInvoiceItem
                    .Where(x => x.ItemInventoryId != null)
                    .Select(x => (int)x.ItemInventoryId)
                    .ToList();

                context.TrnSalesInvoice.Remove(article);

                await context.SaveChangesAsync();

                await Library.Inventory.UpdateArticleInventory(articleInventories);
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

        public async Task<IActionResult> LineArticles([DataSourceRequest] DataSourceRequest request)
        {
            var result = await new FMISContext()
                .MstArticle
                .Where(x => x.ArticleTypeId == 1)
                .OrderBy(x => x.Article)
                .ToListAsync();

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> LineItemInventories([DataSourceRequest] DataSourceRequest request)
        {
            var result = await new FMISContext()
                .MstArticleInventory
                .OrderByDescending(x => x.InventoryCode)
                .ToListAsync();

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineUnits([DataSourceRequest] DataSourceRequest request, int articleId)
        {
            var units = new Queries.RRUnitQ002();
            units.ArticleParam = articleId;

            var result = units.Result();

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult Discounts([DataSourceRequest] DataSourceRequest request)
        {
            var units = new FMISContext().MstDiscount.ToList();

            return Json(units.ToDataSourceResult(request));
        }


        public IActionResult LineTaxes([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstTaxType>();

            using (var context = new FMISContext())
            {
                result = context.MstTaxType.ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineBaseUnits([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstUnit>();

            using (var context = new FMISContext())
            {
                result = context.MstUnit.ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Foreign keys

        #endregion

        #region APIS
        public decimal GetMultiplier(int itemId, int unitId)
        {
            var multiplier = 1m;

            multiplier = new FMISContext().MstArticleUnit.FirstOrDefault(x => x.ArticleId == itemId && x.UnitId == unitId)?.Multiplier ?? 1;

            return multiplier;
        }

        public MstTaxType GetVat(int vatId)
        {
            var result = new MstTaxType();

            result = new FMISContext().MstTaxType.Find(vatId);

            return result;
        }

        public IActionResult ComputeNetPrice(int vatId, int discountId,decimal grossPrice, decimal discountRate) 
        {
            var resultDiscountAmount = 0m;
            var resultNetPrice = 0m;

            var discountIsInclusive = new FMISContext().MstDiscount?.Find(discountId)?.IsInclusive ?? false;

            if (!discountIsInclusive)
            {
                var vatPercentage = new FMISContext().MstTaxType?.Find(vatId)?.TaxRate ?? 0;
                var price = Math.Round(grossPrice / (1 + vatPercentage / 100) ,2);

                resultDiscountAmount = Math.Round(price * (discountRate / 100), 4);
                resultNetPrice = price - resultDiscountAmount;
            }
            else
            {
                resultDiscountAmount = Math.Round(grossPrice * (discountRate/ 100), 4);
                resultNetPrice = grossPrice - resultDiscountAmount;
            }

            return Json(new { DiscountAmount = resultDiscountAmount, NetPrice = resultNetPrice});
        }

        public IActionResult ComputeTax(int vatId, decimal quantity, decimal netPrice, decimal amount)
        {
            var vatAmount = 0m;
            var resultAmount = 0m;
            var vatPercentage = GetVat(vatId).TaxRate;

            var isInclusive = GetVat(vatId)?.IsInclusive ?? false;

            if (isInclusive)
            {
                vatAmount = Math.Round((amount / (1 + (vatPercentage / 100))) * (vatPercentage / 100), 4);
            }
            else
            {
                vatAmount = Math.Round(amount * (vatPercentage / 100), 4);
                resultAmount = Math.Round(quantity * netPrice, 4) + Math.Round(amount * (vatPercentage / 100), 4);
            }

            return Json(new { VatPercentage = vatPercentage, VatAmount = vatAmount, Amount = resultAmount, IsInclusive = isInclusive });
        }

        public IActionResult ComputeBaseUnit(int itemId, int unitId, decimal quantity, decimal amount)
        {
            var resultBaseQuantity = 0m;
            var resultBaseCost = 0m;
            var item = new FMISContext().MstArticle.Find(itemId);
            var multiplier = GetMultiplier(itemId, unitId);

            resultBaseQuantity = quantity / multiplier;

            if (quantity == 0)
            {
                resultBaseCost = 0;
            }
            else
            {
                if (multiplier > 0 )
                {
                    resultBaseCost = Math.Round(amount / (quantity / multiplier), 4);
                }
                else
                {
                    resultBaseCost = 0;
                }
            }

            return Json(new { BaseUnitId = item.UnitId, BaseQuantity = resultBaseQuantity, BasePrice = resultBaseCost });
        }

        public IActionResult GetArticleInventoryId(int branchId, int itemId) 
        {
            return Json(new FMISContext().MstArticleInventory.FirstOrDefault(x => x.BranchId == branchId && x.ArticleId == itemId)?.Id ?? null);
        }

        public IActionResult GetSILineInfos(int itemId, int itemInventoryId,  int unitId, int vatId, int baseUnitId)
        {
            var result = new string[5];

            using (var context = new FMISContext())
            {
                result[0] = context.MstArticle.Find(itemId).Article;
                result[1] = context.MstArticleInventory.Find(itemInventoryId)?.InventoryCode ?? "NA";
                result[2] = context.MstUnit.Find(unitId).Unit;
                result[3] = context.MstTaxType.Find(vatId).TaxType;
                result[4] = context.MstUnit.Find(baseUnitId).Unit;
            }

            return Json(new { Item = result[0], InventoryCode = result[1], Unit = result[2], Vat = result[3], BaseUnit = result[4] });
        }
        #endregion
    }
}
