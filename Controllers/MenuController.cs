using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webfmis.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Index(string search = "")
        {
            return PartialView(new Services.Menu(search).AllActiveMenus);
        }

        [HttpPost]
        public ActionResult Search(string search = "")
        {
            return PartialView("Index", new Services.Menu(search).AllActiveMenus);
        }
    }
}
