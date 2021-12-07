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

namespace webfmis.Controllers
{
    [Authorize]
    public class MstAccountTypeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccountTypeList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstAccountType
                    .OrderBy(x => x.AccountType)
                    .ToDataSourceResult(request);
            }

            return Json(result);
        }
    }
}
