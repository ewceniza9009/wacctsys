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
    public class RepCollectionController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepCollection");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Collection Summary" },
                new ReportList(){ Value = "2", Text = "Collection Detail" },
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
