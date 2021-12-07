using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Mapping;
using webfmis.Models;
using webfmis.Services;

namespace webfmis.Controllers
{
    [Authorize]
    public class MstItemController : Controller
    {
        public const int ARTICLE_TYPE = 1;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MstItemController(IWebHostEnvironment hostEnvironment) 
        {
            webHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstItem");

            return View();
        }

        #region Grid
        public IActionResult ArticleList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstArticle
                    .OrderBy(x => x.Article)
                    .Where(x => x.ArticleTypeId == ARTICLE_TYPE)
                    .ToList()
                    .ToDataSourceResult(request);
            }

            return Json(result);
        }

        public IActionResult ArticleUnitList([DataSourceRequest] DataSourceRequest request, int ArticleId)
        {
            List<MstArticleUnit> result = null;

            using (var context = new FMISContext())
            {
                result = context.MstArticleUnit
                    .Where(x => x.ArticleId == ArticleId)
                    .ToList();

                foreach (var item in result)
                {
                    item.BaseUnitName = new FMISContext().MstUnit
                        .Find(new FMISContext().MstArticle.Find(ArticleId)?.UnitId)?.Unit;
                    item.EqualSign = "=";
                }
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult ArticleComponentList([DataSourceRequest] DataSourceRequest request, int ArticleId)
        {
            List<MstArticleComponent> result = null;

            using (var context = new FMISContext())
            {
                result = context.MstArticleComponent
                    .Where(x => x.ArticleId == ArticleId)
                    .ToList();

                foreach (var item in result)
                {
                    item.ComponentManualCode = new FMISContext().MstArticle.Find(item.ComponentArticleId)?.ManualArticleCode;
                    item.ComponentUnit = new FMISContext().MstArticle.Find(item.ComponentArticleId)?.UnitName;
                    item.Cost = new FMISContext().MstArticle.Find(item.ComponentArticleId).Cost ?? 0;
                    item.Amount = item.Quantity * item.Cost;
                }
            }

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Commands
        public IActionResult Add()
        {
            var code = ArticleCommon.ArticleMaxCode(ARTICLE_TYPE);
            var articleCode = "";
            string itemImage = $@"/itemImages/no-image.png";

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
                Category = "NA",
                AccountId = Common.ArticleAccountId(),
                SalesAccountId = Common.SalesAccountId(),
                CostAccountId = Common.CostAccountId(),
                AssetAccountId = Common.AssetAccountId(),
                ExpenseAccountId = Common.ExpenseAccountId(),
                UnitId = Common.UnitId(),
                OutputTaxId = Common.OutputTaxId(),
                InputTaxId = Common.InputTaxId(),
                WtaxTypeId = Common.WtaxTypeId(),
                Price = 0,
                Cost = 0,
                IsInventory = false,
                Particulars = "NA",
                DateAcquired = DateTime.Now,
                SalvageValue = 0,
                UsefulLife = 0,
                ArticleImagePath = itemImage,
                IsLocked = false
            };

            ArticleCommon.MstArticleDefaults(result, ARTICLE_TYPE);

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> ArticleDetail(int Id)
        {
            MstArticle result = null;

            using (var context = new FMISContext())
            {
                result = await context.MstArticle.FindAsync(Id);

                if (string.IsNullOrEmpty(result.ArticleImage))
                {
                    result.ArticleImagePath = $@"/itemImages/no-image.png";
                }
                else 
                {
                    result.ArticleImagePath = $@"/itemImages/{result.ArticleImage}";
                }

                result.OldArticleImage = result.ArticleImage;
            }

            ViewData["ArticleId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddArticleUnit(int articleId)
        {
            var result = new MstArticleUnit()
            {
                Id = 0,
                ArticleId = articleId,
                UnitId = new FMISContext().MstArticle?.Find(articleId)?.UnitId ?? 0,
                EqualSign = "=",
                BaseUnitName = new FMISContext().MstUnit
                    .Find(new FMISContext().MstArticle.Find(articleId)?.UnitId)?.Unit
            };

            return Json(result);
        }

        public IActionResult AddArticleComponent(int articleId)
        {
            var result = new MstArticleComponent()
            {
                Id = 0,
                ArticleId = articleId,
                ComponentArticleId = Common.DefaultInvetoryArticle().Id,
                Quantity = 1,
                ComponentManualCode = "NA",
                ComponentUnit = "NA",
                Cost = Common.DefaultInvetoryArticle().Cost ?? 0,
                Amount = Common.DefaultInvetoryArticle().Cost ?? 0,
                Particulars = "NA"
            };

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new MstArticle();

            using (var context = new FMISContext())
            {
                var article = Common.GetArticleById(Id);

                if (article != null) 
                {
                    result = context.MstArticle
                        .Where(x => x.Article.CompareTo(article.Article) < 0 && x.ArticleTypeId == ARTICLE_TYPE)
                        .OrderByDescending(x => x.Article)
                        .FirstOrDefault();
                }
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new MstArticle();

            using (var context = new FMISContext())
            {
                var article = Common.GetArticleById(Id);

                if (article != null) 
                {
                    result = context.MstArticle
                        .Where(x => x.Article.CompareTo(article.Article) > 0 && x.ArticleTypeId == ARTICLE_TYPE)
                        .OrderBy(x => x.Article)
                        .FirstOrDefault();
                }
            }

            return Json(result);
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

        [HttpPost]
        public async Task<IActionResult> Save(MstArticle data, List<MstArticleUnit> dataSub, List<MstArticleComponent> dataSub1)
        {
            data.MstArticleUnit = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();
            data.MstArticleComponentArticle = dataSub1.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();

            var mappingProfile = new MappingProfileForMstItem();
            var article = new MstArticle();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    article = await context.MstArticle.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, article);

                    ArticleCommon.MstArticleDefaults(article, ARTICLE_TYPE);

                    await context.MstArticleUnit.AddRangeAsync(article.MstArticleUnit.Where(x => x.Id == 0));
                    context.MstArticleUnit.RemoveRange(article.MstArticleUnit.Where(x => x.IsDeleted));

                    await context.MstArticleComponent.AddRangeAsync(article.MstArticleComponentArticle.Where(x => x.Id == 0));
                    context.MstArticleComponent.RemoveRange(article.MstArticleComponentArticle.Where(x => x.IsDeleted));
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
        #endregion

        #region Combobox
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

        public async Task<IActionResult> ArticleGroups([DataSourceRequest] DataSourceRequest request)
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

        public async Task<IActionResult> Units([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstUnit>();

            using (var context = new FMISContext())
            {
                result = await context.MstUnit
                    .OrderBy(x => x.Unit)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Taxes([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstTaxType>();

            using (var context = new FMISContext())
            {
                result = await context.MstTaxType
                    .OrderBy(x => x.TaxType)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        } 
        #endregion

        #region Autocomplete
        public IActionResult Categories(string text)
        {
            var result = new List<DTO.Category>();

            using (var context = new FMISContext())
            {
                result = context.MstArticle
                    .Where(x => x.Category != "" && x.ArticleTypeId == ARTICLE_TYPE && x.Category.Contains(text))
                    .GroupBy(x => x.Category)
                    .OrderBy(x => x.Key)
                    .Select(x => new DTO.Category() { CategoryName = x.Key })
                    .ToList();
            }

            return Json(result);
        } 
        #endregion

        #region Foreign Keys
        public async Task<IActionResult> ArticleUnits()
        {
            var result = new List<MstUnit>();

            using (var context = new FMISContext())
            {
                result = await context.MstUnit.ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> Articles()
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Article)
                    .Where(x => x.IsInventory).ToListAsync();
            }

            return Json(result);
        }
        #endregion

        #region Methods
        public IActionResult GetArticleById(int articleId)
        {
            return Json(Common.GetArticleById(articleId));
        }
        #endregion

        # region API's
        [HttpPost]
        public IActionResult UploadImage(IFormFile files)
        {
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "itemImages");
            var articleId = int.Parse(Request.Form["articleId"][0]);
            var item = new Models.MstArticle();

            using (var context = new FMISContext())
            {
                item = context.MstArticle.Find(articleId);
                var oldArticleImagePath = item.OldArticleImage;

                var fileName = "";

                if (files != null)
                {
                    fileName = files.FileName;
                }
                else 
                {
                    fileName = oldArticleImagePath;
                }

                if (oldArticleImagePath != fileName && oldArticleImagePath != null) 
                {
                    System.IO.File.Delete(Path.Combine(uploadsFolder, oldArticleImagePath));
                }

                item.ArticleImage = fileName;
                context.SaveChanges();
            }


            if (files != null)
            {
                string itemImagePath = Path.Combine(uploadsFolder, files.FileName);
                
                using (var fileStream = new FileStream(itemImagePath, FileMode.Create))
                {
                    files.CopyTo(fileStream);
                }
                return new ObjectResult(new { status = "success" });
            }
            return new ObjectResult(new { status = "fail" });
        }
        #endregion
    }
}
