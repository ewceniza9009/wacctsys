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
    public class TrnReceivingReceiptController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnReceivingReceipt");

            return View();
        }

        #region Grid
        public IActionResult ReceivingReceiptList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.TrnReceivingReceipt
                    .ToList()
                    .ToDataSourceResult(request);

                result.Data = result.Data.Cast<TrnReceivingReceipt>().OrderByDescending(x => x.Rrnumber);
            }

            return Json(result);
        }

        public IActionResult TrnReceivingReceiptLineList([DataSourceRequest] DataSourceRequest request, int RRId)
        {
            List<TrnReceivingReceiptItem> result = null;

            using (var context = new FMISContext())
            {
                result = context.TrnReceivingReceiptItem
                    .Where(x => x.Rrid == RRId)
                    .ToList();

                foreach (var item in result)
                {
                    item.PONumber = new FMISContext().TrnPurchaseOrder.Find(item.Poid).Ponumber;
                    item.ItemName = new FMISContext().MstArticle.Find(item.ItemId).Article;
                    item.UnitName = new FMISContext().MstUnit.Find(item.UnitId).Unit;
                    item.BranchName = new FMISContext().MstBranch.Find(item.BranchId).Branch;
                    item.VatInfo = $"@{Math.Round(item.Vatpercentage, 2)}% {new FMISContext().MstTaxType.Find(item.Vatid).TaxType} => {Math.Round(item.Vatamount, 2)}";
                    item.WTaxInfo = $"@{Math.Round(item.Wtaxpercentage, 2)}% {new FMISContext().MstTaxType.Find(item.Wtaxid).TaxType} => {Math.Round(item.Wtaxamount, 2)}";
                    item.UnitConversionInfo = $"{Math.Round(item.BaseQuantity, 2)} {new FMISContext().MstUnit.Find(item.BaseUnitId).Unit} Cost: {Math.Round(item.BaseCost, 2)}";
                }
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult POStatusLines([DataSourceRequest] DataSourceRequest request)
        {
            List<DTO.TmpPurchaseOrderStatusItem> result = new List<DTO.TmpPurchaseOrderStatusItem>();

            using (var context = new FMISContext())
            {
                var query =new Queries.RRPOStatusQ002();

                if (request.Filters.Count > 0) 
                {
                    var filter = request.Filters[0] as Kendo.Mvc.FilterDescriptor;

                    if (!string.IsNullOrEmpty(filter.Value.ToString())) 
                    {
                        query.ParamId = filter.Member == "POId" ? int.Parse(filter.Value.ToString()) : 0;
                        query.ReadPOStatus();

                        result = query.Result();
                    }
                }
            }

            if (result.Count == 0) 
            {
                return Json(result);
            }

            return Json(result.ToDataSourceResult(request));
        }

        #endregion

        #region Commands
        public IActionResult Add()
        {
            var settings = TransactionCommon.GetCurrentSettings();

            var code = TransactionCommon.RRMaxCode(settings.BranchId);
            var rrCode = "";

            if (code != "0000000000" && code != null)
            {
                rrCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                rrCode = "0000000001";
            }

            var result = new TrnReceivingReceipt()
            {
                BranchId = External.ReadSettings().Id,
                Rrnumber = rrCode,
                Rrdate = DateTime.Now.Date,
                SupplierId = TransactionCommon.DefaultSupplierId(settings.BranchId),
                TermId = Common.DefaultTermId(),
                DocumentReference = "NA",
                Remarks = "NA",
                ManualRrnumber = "NA",
                ReceivedById = settings.CurrentUserId,
                PreparedById = settings.CurrentUserId,
                CheckedById = settings.CurrentUserId,
                ApprovedById = settings.CurrentUserId,
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> ReceivingReceiptDetail(int Id)
        {
            TrnReceivingReceipt result = null;

            using (var context = new FMISContext())
            {
                result = await context.TrnReceivingReceipt.FindAsync(Id);
            }

            ViewData["RRId"] = Id;

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> DownloadPO(int Id, int supplierId)
        {
            TrnReceivingReceipt result = null;

            if (Id == 0)
            {
                result = new TrnReceivingReceipt()
                {
                    SupplierId = supplierId
                };
            }
            else 
            {
                using (var context = new FMISContext())
                {
                    result = await context.TrnReceivingReceipt.FindAsync(Id);
                }
            }

            return PartialView("DownloadPO", result);
        }

        public IActionResult AddRRLine(int rrId, int supplierId)
        {
            var branch = External.ReadSettings();
            var latestPO = new FMISContext()
                .TrnPurchaseOrder
                .Include(x => x.TrnPurchaseOrderItem)
                .OrderByDescending(x => x.Ponumber)
                .FirstOrDefault(x => x.BranchId == branch.Id && x.SupplierId == supplierId && !x.IsClose);
            var line = latestPO.TrnPurchaseOrderItem.FirstOrDefault();
            var item = new FMISContext().MstArticle.FirstOrDefault(x => x.Id == line.ItemId);

            var result = new TrnReceivingReceiptItem()
            {
                Id = 0,
                Rrid = rrId,
                Poid = latestPO.Id,
                ItemId = line.ItemId,
                Particulars = "NA",
                Quantity = 0,
                UnitId = line.UnitId,
                Cost = line.Cost,
                Amount = line.Cost,
                BranchId = (int)branch.Id,
                BranchName = branch.Branch,
                Vatid = Common.InputTaxId(),
                Vatpercentage = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id== Common.InputTaxId())?.TaxRate ?? 0,
                Vatamount = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id== Common.InputTaxId())?.TaxRate ?? 0,
                Wtaxid = Common.WtaxTypeId(),
                Wtaxpercentage = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id == Common.WtaxTypeId())?.TaxRate ?? 0,
                Wtaxamount = new FMISContext().MstTaxType.FirstOrDefault(x => x.Id == Common.WtaxTypeId())?.TaxRate ?? 0,
                BaseUnitId = item.UnitId,
                BaseQuantity = 0,
                BaseCost = 0,
                VatInfo = "Select PO",
                WTaxInfo = "Select PO",
                UnitConversionInfo = "Select PO"
            };

            ViewData["SupplierId"] = supplierId;

            return PartialView("LineDetail", result);
        }

        [HttpPost]
        public IActionResult DetailRRLine(TrnReceivingReceiptItem rrLine) 
        {
            rrLine.PONumber = new FMISContext().TrnPurchaseOrder.Find(rrLine.Poid).Ponumber;
            rrLine.ItemName = new FMISContext().MstArticle.Find(rrLine.ItemId).Article;
            rrLine.UnitName = new FMISContext().MstUnit.Find(rrLine.UnitId).Unit;
            rrLine.BranchName = new FMISContext().MstBranch.Find(rrLine.BranchId).Branch;
            rrLine.VatInfo = $"@{Math.Round(rrLine.Vatpercentage, 2)}% {new FMISContext().MstTaxType.Find(rrLine.Vatid).TaxType} => {Math.Round(rrLine.Vatamount, 2)}";
            rrLine.WTaxInfo = $"@{Math.Round(rrLine.Wtaxpercentage, 2)}% {new FMISContext().MstTaxType.Find(rrLine.Wtaxid).TaxType} => {Math.Round(rrLine.Wtaxamount, 2)}";
            rrLine.UnitConversionInfo = $"{Math.Round(rrLine.BaseQuantity, 2)} {new FMISContext().MstUnit.Find(rrLine.BaseUnitId).Unit} Cost: {Math.Round(rrLine.BaseCost, 2)}";

            return PartialView("LineDetail", rrLine);
        }

        public IActionResult Prev(int Id)
        {
            var result = new TrnReceivingReceipt();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetRRById(Id);

                result = context.TrnReceivingReceipt
                    .Where(x => x.Rrnumber.CompareTo(entity.Rrnumber) < 0)
                    .OrderByDescending(x => x.Rrnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new TrnReceivingReceipt();

            using (var context = new FMISContext())
            {
                var entity = TransactionCommon.GetRRById(Id);

                result = context.TrnReceivingReceipt
                    .Where(x => x.Rrnumber.CompareTo(entity.Rrnumber) > 0)
                    .OrderBy(x => x.Rrnumber)
                    .FirstOrDefault();
            }

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Save(TrnReceivingReceipt data, List<TrnReceivingReceiptItem> dataSub)
        {
            data.TrnReceivingReceiptItem = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList(); ;

            var mappingProfile = new MappingProfileForTrnReceivingReceipt();
            var entity = new TrnReceivingReceipt();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    entity = await context.TrnReceivingReceipt.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, entity);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnReceivingReceiptItem.AddRangeAsync(entity.TrnReceivingReceiptItem.Where(x => x.Id == 0 && x.IsDeleted == false));
                    context.TrnReceivingReceiptItem.RemoveRange(entity.TrnReceivingReceiptItem.Where(x => x.IsDeleted));
                }
                else
                {
                    entity = mappingProfile.mapper.Map<TrnReceivingReceipt>(data);

                    TransactionCommon.TransactionDefaults(entity);

                    await context.TrnReceivingReceipt.AddAsync(entity);
                }

                entity.Amount = entity.TrnReceivingReceiptItem.Sum(x => x.Amount);
                entity.WtaxAmount = entity.TrnReceivingReceiptItem.Sum(x => x.Wtaxamount);

                await context.SaveChangesAsync();

                await Library.Inventory.InsertRRInventory(entity);

                await Library.Journal.InsertRRJournal(entity);

                Library.Balances.UpdateAP(entity.Id);
            }

            return Json(new { Id = entity.Id, RRNumber = entity.Rrnumber });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.TrnReceivingReceipt
                    .FirstOrDefault(x => x.Id == Id);

                context.TrnReceivingReceipt.Remove(article);

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

        public async Task<IActionResult> LinePOs([DataSourceRequest] DataSourceRequest request, int supplierId)
        {
            var result = new List<TrnPurchaseOrder>();

            using (var context = new FMISContext())
            {
                result = await context.TrnPurchaseOrder
                    .OrderByDescending(x => x.Ponumber)
                    .Where(x => x.BranchId == External.ReadSettings().Id && x.SupplierId == supplierId && !x.IsClose)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> LineArticles([DataSourceRequest] DataSourceRequest request, int poId)
        {
            var articles = new Queries.RRItemQ002();
            articles.POParam = poId;

            var result = await articles.Result();

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineUnits([DataSourceRequest] DataSourceRequest request, int articleId)
        {
            var units = new Queries.RRUnitQ002();
            units.ArticleParam = articleId;

            var result = units.Result();

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult LineBranches([DataSourceRequest] DataSourceRequest request)
        {
            var result = new  List<MstBranch>();

            using (var context = new FMISContext()) 
            {
                result = context.MstBranch.ToList();
            }

            return Json(result.ToDataSourceResult(request));
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

        public IActionResult GetItemCost(int poId, int itemId) 
        {
            var result = new FMISContext().TrnPurchaseOrderItem?.FirstOrDefault(x => x.Poid == poId && x.ItemId == itemId)?.Cost ?? 0;

            return Json(new { Cost = result });
        }

        public IActionResult ComputeTax(int vatId, int wTaxId, decimal quantity, decimal cost, decimal amount) 
        {
            var vatAmount = 0m;
            var wTaxAmount = 0m;
            var resultAmount = 0m;

            var vatPercentage = GetVat(vatId).TaxRate;
            var wTaxPercentage = GetVat(wTaxId).TaxRate;

            var isInclusive = GetVat(vatId)?.IsInclusive ?? false;

            if (isInclusive)
            {
                vatAmount = Math.Round((amount / (1 + (vatPercentage / 100))) * (vatPercentage / 100), 4);
                wTaxAmount = Math.Round((amount / (1 + (vatPercentage / 100))) * (wTaxPercentage / 100), 4);
            }
            else 
            {
                vatAmount = Math.Round(amount * (vatPercentage / 100), 4);
                wTaxAmount = Math.Round(amount * (wTaxPercentage / 100), 4);
                resultAmount = Math.Round(quantity * cost, 4) + Math.Round(amount * (vatPercentage / 100), 4);
            }

            return Json(new { VatPercentage = vatPercentage, WTaxPercentage = wTaxPercentage, VatAmount = vatAmount, WTaxAmount = wTaxAmount, Amount = resultAmount, IsInclusive = isInclusive });
        }

        public IActionResult ComputeBaseUnit(int itemId, int unitId, int vatId, decimal quantity, decimal amount, decimal vatAmount)
        {
            var resultBaseQuantity = 0m;
            var resultBaseCost = 0m;
            var item = new FMISContext().MstArticle.Find(itemId);

            var multiplier = GetMultiplier(itemId, unitId);
            var isInclusive = GetVat(vatId)?.IsInclusive ?? false;

            resultBaseQuantity = quantity / multiplier;

            if (quantity == 0)
            {
                resultBaseCost = 0;
            }
            else 
            {
                if (isInclusive)
                {
                    resultBaseCost = Math.Round((amount - vatAmount) / (quantity / multiplier), 4);
                }
                else 
                {
                    resultBaseCost = Math.Round(amount / (quantity / multiplier), 4);
                }
            }

            return Json(new { BaseUnitId = item.UnitId, BaseQuantity = resultBaseQuantity, BaseCost = resultBaseCost});
        }

        public IActionResult GetRRLineInfos(int pOId, int itemId, int unitId, int branchId, int vatId, int wTaxId, int baseUnitId) 
        {
            var result = new string[7];

            using (var context = new FMISContext()) 
            {
                result[0] = context.TrnPurchaseOrder.Find(pOId).Ponumber;
                result[1] = context.MstArticle.Find(itemId).Article;
                result[2] = context.MstUnit.Find(unitId).Unit;
                result[3] = context.MstBranch.Find(branchId).Branch;
                result[4] = context.MstTaxType.Find(vatId).TaxType;
                result[5] = context.MstTaxType.Find(wTaxId).TaxType;
                result[6] = context.MstUnit.Find(baseUnitId).Unit;
            }

            return Json(new { PONumber = result[0], Item = result[1], Unit = result[2], Branch = result[3], Vat = result[4], WTax = result[5], BaseUnit = result[6] });
        }

        [HttpPost]
        public IActionResult DownloadPOLinesSelected(int rrId, List<DTO.TmpPurchaseOrderStatusItem> poLinesSelected) 
        {
            var result = new List<TrnReceivingReceiptItem>();

            foreach (var line in poLinesSelected.Where(x => x?.Pick ?? false)) 
            {
                var branch = External.ReadSettings();
                var vat = new FMISContext().MstTaxType.Find(Common.InputTaxId());
                var wTax = new FMISContext().MstTaxType.Find(Common.WtaxTypeId());

                var vatAmount = 0m;
                var wTaxAmount = 0m;
                var resultAmount = line.Cost * line.Quantity;

                var isInclusive = vat?.IsInclusive ?? false;

                if (isInclusive)
                {
                    vatAmount = Math.Round((resultAmount / (1 + (vat.TaxRate / 100))) * (vat.TaxRate / 100), 4);
                    wTaxAmount = Math.Round((resultAmount / (1 + (wTax.TaxRate / 100))) * (wTax.TaxRate / 100), 4);
                }
                else
                {
                    vatAmount = Math.Round(resultAmount * (vat.TaxRate / 100), 4);
                    wTaxAmount = Math.Round(resultAmount * (wTax.TaxRate / 100), 4);
                    resultAmount = Math.Round(line.Quantity * resultAmount, 4) + Math.Round(resultAmount * (vat.TaxRate / 100), 4);
                }

                var resultBaseQuantity = 0m;
                var resultBaseCost = 0m;

                var multiplier = GetMultiplier(line.ItemId, line.UnitId);

                resultBaseQuantity = line.Quantity / multiplier;

                if (line.Quantity == 0)
                {
                    resultBaseCost = 0;
                }
                else
                {
                    if (isInclusive)
                    {
                        resultBaseCost = Math.Round((resultAmount - vatAmount) / (line.Quantity / multiplier), 4);
                    }
                    else
                    {
                        resultBaseCost = Math.Round(resultAmount / (line.Quantity / multiplier), 4);
                    }
                }

                var item = new FMISContext().MstArticle.Find(line.ItemId);

                result.Add(new TrnReceivingReceiptItem()
                {
                    Id = 0,
                    Rrid = rrId,
                    Poid = line.POId,
                    PONumber = new FMISContext().TrnPurchaseOrder.Find(line.POId).Ponumber,
                    ItemId = line.ItemId,
                    ItemName = line.ItemName,
                    Particulars = "NA",
                    Quantity = line.Quantity,
                    UnitId = line.UnitId,
                    UnitName = line.UnitName,
                    Cost = line.Cost,
                    Amount = resultAmount,
                    BranchId = (int)branch.Id,
                    BranchName = branch.Branch,
                    Vatid = vat.Id,
                    Vatpercentage = vat?.TaxRate ?? 0,
                    Vatamount = vatAmount,
                    Wtaxid = wTax.Id,
                    Wtaxpercentage = wTax?.TaxRate ?? 0,
                    Wtaxamount = wTaxAmount,
                    BaseUnitId = line.UnitId,
                    BaseQuantity = resultBaseQuantity,
                    BaseCost = resultBaseCost,
                    VatInfo = $"@{Math.Round(vat.TaxRate, 2)}% {vat.TaxType} => {Math.Round(vatAmount, 2)}",
                    WTaxInfo = $"@{Math.Round(wTax.TaxRate, 2)}% {wTax.TaxType} => {Math.Round(wTaxAmount, 2)}",
                    UnitConversionInfo = $"{Math.Round(resultBaseQuantity, 2)} {new FMISContext().MstUnit.Find(item.UnitId).Unit} Cost: {Math.Round(resultBaseCost, 2)}"
                }); ;
            }

            return Json(new { Result = result});
        }
        #endregion
    }
}
