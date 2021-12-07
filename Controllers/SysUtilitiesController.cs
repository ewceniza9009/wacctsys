using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webfmis.Controllers
{
    [Authorize]
    public class SysUtilitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
