using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.DTO;
using webfmis.Models;

namespace webfmis.Controllers
{
    [Authorize]
    public class RepInventoryController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepInventory");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Inventory" },
                new ReportList(){ Value = "2", Text = "Stock Card" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Stock In Detail Report" },
                new ReportList(){ Value = "5", Text = "Stock Out Detail Report" },
                new ReportList(){ Value = "6", Text = "Stock Transfer Detail Report" },
                new ReportList(){ Value = "7", Text = "" },
                new ReportList(){ Value = "8", Text = "Item List" },
                new ReportList(){ Value = "9", Text = "Item Component List" },
                new ReportList(){ Value = "10", Text = "" },
                new ReportList(){ Value = "11", Text = "Physical Count Sheet" },
                new ReportList(){ Value = "12", Text = "" },
                new ReportList(){ Value = "13", Text = "Fixed Asset Report" },
            };

            return View();
        }

        public async Task<IActionResult> Companies([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstCompany>();

            using (var context = new FMISContext())
            {
                result = await context.MstCompany
                    .OrderBy(x => x.Company)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Items([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Article)
                    .Where(x => x.ArticleTypeId == 1)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }


        public async Task<IActionResult> StockCounts([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<TrnStockCount>();

            using (var context = new FMISContext())
            {
                result = await context.TrnStockCount
                    .OrderByDescending(x => x.Scnumber)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
    }
}
