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
    public class RepAccountsPayableController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepAccountsPayable");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Accounts Payable" },
                new ReportList(){ Value = "2", Text = "" },
                new ReportList(){ Value = "3", Text = "Accounts Payable Voucher" },
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

        public async Task<IActionResult> RRNos([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<TrnReceivingReceipt>();

            using (var context = new FMISContext())
            {
                result = await context.TrnReceivingReceipt
                    .OrderByDescending(x => x.Rrnumber)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
    }
}
