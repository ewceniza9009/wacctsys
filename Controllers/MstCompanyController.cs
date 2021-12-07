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
    public class MstCompanyController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstCompany");

            return View();
        }

        public IActionResult CompanyList([DataSourceRequest] DataSourceRequest request)
        {
            List<MstCompany> result = null;

            using (var context = new FMISContext()) 
            {
                result = context.MstCompany
                    .OrderBy(x => x.Company)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult BranchList([DataSourceRequest] DataSourceRequest request, int CompanyId)
        {
            List<MstBranch> result = null;

            using (var context = new FMISContext())
            {
                result = context.MstBranch
                    .OrderBy(x => x.Branch)
                    .Where(x => x.CompanyId == CompanyId)
                    .ToList();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult Add() 
        {
            var result = new MstCompany()
            {
                Id = 0,
                Company = "NA",
                Address = "NA",
                ContactNumber = "NA",
                TaxNumber = "NA",
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> CompanyDetail(int Id)
        {
            MstCompany result = null;

            using (var context = new FMISContext())
            {
                result = await context.MstCompany.FindAsync(Id);
            }

            ViewData["CompanyId"] = Id;

            return PartialView("Detail", result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MstCompany data, List<MstBranch> dataSub) 
        {
            data.MstBranch = dataSub;

            var mappingProfile = new MappingProfileForMstCompany();
            var company = new MstCompany();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    company = await context.MstCompany.FindAsync(data.Id);

                    foreach (var line in data.MstBranch)
                    {
                        MstBranchDefaults(line);
                    }

                    mappingProfile.mapper.Map(data, company);
                    MstCompanyDefaults(company);

                    await context.MstBranch.AddRangeAsync(company.MstBranch.Where(x => x.Id == 0));
                    context.MstBranch.RemoveRange(company.MstBranch.Where(x => x.IsDeleted));
                }
                else 
                {
                    foreach (var line in data.MstBranch)
                    {
                        MstBranchDefaults(line);
                    }

                    company = mappingProfile.mapper.Map<MstCompany>(data);
                    MstCompanyDefaults(company);

                    await context.MstCompany.AddAsync(company);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = company.Id, Company = company.Company});
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext()) 
            {
                var company = context.MstCompany
                    .Include(x => x.MstBranch)
                    .FirstOrDefault(x => x.Id == Id);

                context.MstCompany.Remove(company);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }

        public IActionResult Prev(int Id) 
        {
            var result = new MstCompany();

            using (var context = new FMISContext())
            {
                var company = Common.GetCompanyById(Id);

                result = context.MstCompany
                    .Where(x => x.Company.CompareTo(company.Company) < 0)
                    .OrderByDescending(x => x.Company)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new MstCompany();

            using (var context = new FMISContext())
            {
                var company = Common.GetCompanyById(Id);

                result = context.MstCompany
                    .Where(x => x.Company.CompareTo(company.Company) > 0)
                    .OrderBy(x => x.Company)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        private static void MstCompanyDefaults(MstCompany company)
        {
            company.IsLocked = true;
            company.CreatedById = 1;
            company.CreatedDateTime = DateTime.Now;
            company.UpdatedById = 1;
            company.UpdatedDateTime = DateTime.Now;
        }

        private static void MstBranchDefaults(MstBranch branch)
        {
            branch.IsLocked = true;
            branch.CreatedById = 1;
            branch.CreatedDateTime = DateTime.Now;
            branch.UpdatedById = 1;
            branch.UpdatedDateTime = DateTime.Now;
        }
    }
}
