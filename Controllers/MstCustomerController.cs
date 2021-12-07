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
    public class MstCustomerController : Controller
    {
        public const int ARTICLE_TYPE = 2;

        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstCustomer");

            return View();
        }

        public IActionResult ArticleList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstArticle
                    .OrderBy(x => x.Article)
                    .Where(x => x.ArticleTypeId == ARTICLE_TYPE)
                    .ToDataSourceResult(request);
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
                ArticleGroupId = null,
                AccountId = Common.ArticleAccountId(),
                TermId = Common.DefaultTermId(),
                Address = "NA",
                ContactNumber = "NA",
                ContactPerson = "NA",
                TaxNumber = "NA",
                Particulars = "NA",
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> ArticleDetail(int Id)
        {
            MstArticle result = null;

            using (var context = new FMISContext())
            {
                result = await context.MstArticle.FindAsync(Id);
            }

            return PartialView("Detail", result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MstArticle data)
        {

            var userRights = Library.Security.GetFormRights("MstCustomer");

            if (!userRights.CanLock) 
            {
                throw new ArgumentException($"The process cannot continue!!!.");
            }

            var mappingProfile = new MappingProfile<MstArticle, MstArticle>();
            var article = new MstArticle();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    article = await context.MstArticle.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, article);
                    ArticleCommon.MstArticleDefaults(article, ARTICLE_TYPE);
                }
                else
                {
                    article = mappingProfile.mapper.Map<MstArticle>(data);
                    ArticleCommon.MstArticleDefaults(article, ARTICLE_TYPE);

                    await context.MstArticle.AddAsync(article);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = article.Id, Article = article.Article });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var article = context.MstArticle
                    .FirstOrDefault(x => x.Id == Id);

                context.MstArticle.Remove(article);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new MstArticle();

            using (var context = new FMISContext())
            {
                var article = Common.GetArticleById(Id);

                result = context.MstArticle
                    .Where(x => x.Article.CompareTo(article.Article) < 0 && x.ArticleTypeId == ARTICLE_TYPE)
                    .OrderByDescending(x => x.Article)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new MstArticle();

            using (var context = new FMISContext())
            {
                var article = Common.GetArticleById(Id);

                result = context.MstArticle
                    .Where(x => x.Article.CompareTo(article.Article) > 0 && x.ArticleTypeId == ARTICLE_TYPE)
                    .OrderBy(x => x.Article)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public async Task<IActionResult> CustomerGroups([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticleGroup>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticleGroup
                    .OrderBy(x => x.ArticleGroup)
                    .Where(x => x.ArticleTypeId == ARTICLE_TYPE)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Accounts([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstAccount>();

            using (var context = new FMISContext())
            {
                result =  await context.MstAccount
                    .OrderBy(x => x.Account)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Terms([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstTerm>();

            using (var context = new FMISContext()) 
            {
                result = await context.MstTerm
                    .OrderBy(x => x.Term)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
    }
}
