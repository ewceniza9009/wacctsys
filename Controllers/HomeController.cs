using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.DTO;
using webfmis.Models;
using webfmis.Utils;

namespace webfmis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var visitorData = new List<VisitorConversionViewModel>()
            {
                new VisitorConversionViewModel(){ Value = 2000, Date = new DateTime(2021, 11, 1), Channel = "Organic Search", Conversion = 8232, Users = 70500 },
                new VisitorConversionViewModel(){ Value = 80000, Date = new DateTime(2021, 11, 5), Channel = "Direct", Conversion = 6574, Users = 24900 },
                new VisitorConversionViewModel(){ Value = 130000, Date = new DateTime(2021, 11, 10),Channel = "Referral", Conversion = 4932, Users = 20000 },
                new VisitorConversionViewModel(){ Value = 170000, Date = new DateTime(2021, 11, 15),Channel = "Social Media", Conversion = 2928, Users = 19500 },
                new VisitorConversionViewModel(){ Value = 190000, Date = new DateTime(2021, 11, 20),Channel = "Email", Conversion = 2456, Users = 18100 },
                new VisitorConversionViewModel(){ Value = 190000, Date = new DateTime(2021, 11, 30),Channel = "Other", Conversion = 1172, Users = 16540 }
            };

            bool fileExist = System.IO.File.Exists(LocalDB.DBFile);

            if (!fileExist) 
            {
                LocalDB.CreateTempDBFile();
            }

            LocalDB.CreateInvoiceWIPTable();
            LocalDB.CreateStockInWIPTable();
            LocalDB.CreatePurchaseOrderStatusTable();
            LocalDB.CreateReceivingStatusTable();
            LocalDB.CreateInvoiceStatusTable();

            var aspUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = new FMISContext().MstUser.FirstOrDefault(x => x.ASPUserId == aspUserId)?.Id;

            Library.Security.GetUserRights(userId ?? 0);

            return View(visitorData);
        }

        [HttpPost]
        public IActionResult TopCustomers() 
        {
            IList<DTO.TopCustomerModel> result = new Queries.HomeTopCustomers().Result();

            return Json(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> BranchList([DataSourceRequest] DataSourceRequest request) 
        {
            var result = new List<MstBranch>();

            using (var context = new FMISContext())
            {
                result = await context.MstBranch
                    .OrderBy(x => x.Company)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult DisplayUser() 
        {
            var result = "";

            using (var context = new FMISContext()) 
            {
                result = context.MstUser.FirstOrDefault(x => x.ASPUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))?.FullName;
            }

            return Json(result);
        }

        public void WriteSettings(int Id) 
        {
            var userId = new FMISContext()
                .MstUser
                .FirstOrDefault(x => x.ASPUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Id;
            External.WriteSettings(Id, userId, false);
        }

        public IActionResult ReadSettings()
        {
            var result = External.ReadSettings();

            return Json(result);
        }

        public IActionResult AdminSwitch(bool isSwitched) 
        {
            External.WriteSettings(External.ReadSettings().Id, External.GetCurrentUserId(), isSwitched);

            return Json(null);
        }

        public IActionResult CurrentAdminSwitch() 
        {
            return Json(External.GetCurrentSwitch());
        }

        public IActionResult GetTotalSales() 
        {
            var result = 0m;

            using (var context = new FMISContext())
            {
                result = context.TrnSalesInvoice.Sum(x => x.Amount);
            }

            return Json(result);
        }
    }
}
