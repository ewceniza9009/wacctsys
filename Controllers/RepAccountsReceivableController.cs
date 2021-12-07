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
    public class RepAccountsReceivableController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepAccountsReceivable");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Accounts Receivable" },
                new ReportList(){ Value = "2", Text = "Accounts Receivable Summary" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Statement Of Accounts" },
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

        public async Task<IActionResult> Customers([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .Where(x => x.ArticleTypeId == 2)
                    .OrderBy(x => x.Article)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
    }
}
