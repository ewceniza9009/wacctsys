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
using webfmis.Services;

namespace webfmis.Controllers
{
    [Authorize]
    public class MstBankController : Controller
    {
        public const int ARTICLE_TYPE = 5;

        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstBank");

            return View();
        }

        public IActionResult ArticleList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstArticle.Where(x => x.ArticleTypeId == ARTICLE_TYPE).ToDataSourceResult(request);
            }

            return Json(result);
        }

        public IActionResult Add()
        {
            var code = ArticleCommon.ArticleMaxCode(ARTICLE_TYPE);
            var articleCode = "";

            if (code != "0000000000")
            {
                articleCode = (long.Parse(code) + 10000000001).ToString().Substring(1, 10);
            }
            else
            {
                articleCode = "0000000001";
            }

            var result = new MstArticle()
            {
                Id = 0,
                ArticleCode = articleCode,
                Article = "NA",
                ArticleGroupId = ArticleCommon.ArticleDefaultGroupId(ARTICLE_TYPE),
                AccountId = ArticleCommon.ArticleDefaultAccountId(ARTICLE_TYPE),                
                Address = "NA",
                ContactNumber = "NA",
                ContactPerson = "NA"
            };

            ArticleCommon.MstArticleDefaults(result, ARTICLE_TYPE);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<MstArticle> data)
        {
            var mappingProfile = new MappingProfile<MstArticle, MstArticle>();
            
            using (var context = new FMISContext())
            {
                foreach (var line in data)
                {
                    var article = context.MstArticle.Find(line.Id);

                    ArticleCommon.MstArticleDefaults(line, ARTICLE_TYPE);

                    if (line.Id == 0) 
                    {
                        await context.MstArticle.AddRangeAsync(line);
                    }

                    if (line.Id > 0)
                    {
                        mappingProfile.mapper.Map(line, article);
                    }

                    if (line.IsDeleted) 
                    {
                        context.MstArticle.Remove(article);
                    }
                }

                await context.SaveChangesAsync();
            }

            return Json(true);
        }
        public async Task<IActionResult> ArticleGroups()
        {
            var result = new List<MstArticleGroup>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticleGroup
                    .Where(x => x.ArticleTypeId == ARTICLE_TYPE)
                    .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> Accounts()
        {
            var result = new List<MstAccount>();

            using (var context = new FMISContext())
            {
                result = await context.MstAccount.OrderBy(x => x.Account).ToListAsync();
            }

            return Json(result);
        }
    }
}
