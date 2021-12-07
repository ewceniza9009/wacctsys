using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Models;

namespace webfmis.Controllers
{
    [Authorize]
    public class TrnJournalController : Controller
    {
        public IActionResult Index(string TrsType, int Id)
        {
            return PartialView(new DTO.JournalModel { TrsType = TrsType, Id = Id});
        }

        public IActionResult JournalList([DataSourceRequest] DataSourceRequest request, string TrsType, int Id)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                if (TrsType == "JV") 
                {
                    result = context.TrnJournal
                        .Where(x => x.Jvid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "RR")
                {
                    result = context.TrnJournal
                        .Where(x => x.Rrid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "CV") 
                {
                    result = context.TrnJournal
                        .Where(x => x.Cvid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "SI")
                {
                    result = context.TrnJournal
                        .Where(x => x.Siid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "OR")
                {
                    result = context.TrnJournal
                        .Where(x => x.Orid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "IN")
                {
                    result = context.TrnJournal
                        .Where(x => x.Inid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "OT")
                {
                    result = context.TrnJournal
                        .Where(x => x.Otid == Id)
                        .ToDataSourceResult(request);
                }

                if (TrsType == "ST")
                {
                    result = context.TrnJournal
                        .Where(x => x.Stid == Id)
                        .ToDataSourceResult(request);
                }

                result.Data = result.Data.Cast<TrnJournal>()
                    .OrderByDescending(x => x.DebitAmount)
                    .ThenByDescending(x => x.CreditAmount);
            }

            return Json(result);
        }
    }
}
