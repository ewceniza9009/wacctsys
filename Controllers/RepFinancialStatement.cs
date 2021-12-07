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
    public class RepFinancialStatement : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("RepFinancialStatement");

            ViewBag.Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Balance Sheet" },
                new ReportList(){ Value = "2", Text = "Income Statement" },
                new ReportList(){ Value = "3", Text = "Cash Flow" },
                new ReportList(){ Value = "4", Text = "" },
                new ReportList(){ Value = "5", Text = "Trial Balance" },
                new ReportList(){ Value = "6", Text = "Account Ledger" },
                new ReportList(){ Value = "7", Text = "" },
                new ReportList(){ Value = "8", Text = "Receiving Receipt Book" },
                new ReportList(){ Value = "9", Text = "Disbursement Book" },
                new ReportList(){ Value = "10", Text = "Sales Book" },
                new ReportList(){ Value = "11", Text = "Collection Book" },
                new ReportList(){ Value = "12", Text = "Stock In Book" },
                new ReportList(){ Value = "13", Text = "Stock Out Book" },
                new ReportList(){ Value = "14", Text = "Stock Transfer Book" },
                new ReportList(){ Value = "15", Text = "Journal Voucher Book" },
                new ReportList(){ Value = "16", Text = "" },
                new ReportList(){ Value = "17", Text = "Chart of Accounts" },
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
    }
}
