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
    public class RepPurchaseController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepPurchases");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Purchase Order Summary" },
                new ReportList(){ Value = "2", Text = "Purchase Order Detail" },
                new ReportList(){ Value = "3", Text = "Purchase Order Status" },
                new ReportList(){ Value = "4", Text = "" },
                new ReportList(){ Value = "5", Text = "Receiving Receipt Summary" },
                new ReportList(){ Value = "6", Text = "Receiving Receipt Detail" },
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
    }
}
