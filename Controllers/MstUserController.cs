using Dapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Mapping;
using webfmis.Models;
using webfmis.Services;

namespace webfmis.Controllers
{
    [Authorize]
    public class MstUserController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("MstUser");

            return View();
        }

        public IActionResult UserList([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstUser
                    .OrderBy(x => x.UserName)
                    .ToDataSourceResult(request);
            }

            return Json(result);
        }

        public IActionResult MstUserFormList([DataSourceRequest] DataSourceRequest request, int userId)
        {
            var result = new DataSourceResult();

            using (var context = new FMISContext())
            {
                result = context.MstUserForm
                    .Include(x => x.Form)
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.Form.Particulars)
                    .ToDataSourceResult(request);
            }

            return Json(result);
        }

        public IActionResult Add()
        {
            var result = new MstUser()
            {
                Id = 0,
                UserName = "NA",
                FullName = "NA",
                Password = "NA",
                IsLocked = false
            };

            return PartialView("Detail", result);
        }

        public async Task<IActionResult> UserDetail(int Id)
        {
            MstUser result = null;

            using (var context = new FMISContext())
            {
                result = await context.MstUser.FindAsync(Id);
            }

            ViewData["UserId"] = Id;

            return PartialView("Detail", result);
        }

        public IActionResult AddLine(int UserId) 
        {
            var result = new MstUserForm()
            {
                Id = 0,
                UserId = UserId,
                FormId = new FMISContext().SysForm.FirstOrDefault().Id,
                CanAdd = true,
                CanEdit = true,
                CanDelete = true,
                CanLock = true,
                CanUnlock = true,
                CanPrint = true,
                IsDeleted = false
            };

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MstUser data, List<MstUserForm> dataSub)
        {
            var aspUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            data.MstUserForm = dataSub.Where(x => !(x.Id == 0 && x.IsDeleted)).ToList();
            data.ASPUserId = aspUserId;

            var userRights = Library.Security.GetFormRights("MstUser");

            if (!userRights.CanLock)
            {
                throw new ArgumentException($"The process cannot continue!!!.");
            }

            var mappingProfile = new MappingProfile<MstUser, MstUser>();
            var user = new MstUser();

            using (var context = new FMISContext())
            {
                if (data.Id > 0)
                {
                    user = await context.MstUser.FindAsync(data.Id);

                    mappingProfile.mapper.Map(data, user);
                    Common.MstUserDefaults(user);
                }
                else
                {
                    user = mappingProfile.mapper.Map<MstUser>(data);
                    Common.MstUserDefaults(user);

                    await context.MstUser.AddAsync(user);
                }

                await context.SaveChangesAsync();
            }

            return Json(new { Id = user.Id, User = user.UserName });
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var result = false;

            using (var context = new FMISContext())
            {
                var User = context.MstUser
                    .FirstOrDefault(x => x.Id == Id);

                context.MstUser.Remove(User);

                await context.SaveChangesAsync();
            }

            return Json(result);
        }

        public IActionResult Prev(int Id)
        {
            var result = new MstUser();

            using (var context = new FMISContext())
            {
                var User = Common.GetUserById(Id);

                result = context.MstUser
                    .Where(x => x.UserName.CompareTo(User.UserName) < 0)
                    .OrderByDescending(x => x.UserName)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public IActionResult Next(int Id)
        {
            var result = new MstUser();

            using (var context = new FMISContext())
            {
                var User = Common.GetUserById(Id);

                result = context.MstUser
                    .Where(x => x.UserName.CompareTo(User.UserName) > 0)
                    .OrderBy(x => x.UserName)
                    .FirstOrDefault();
            }

            return Json(result);
        }

        public async Task<IActionResult> Forms()
        {
            var result = new List<SysForm>();

            using (var context = new FMISContext())
            {
                result = await context
                       .SysForm
                       .OrderBy(x => x.FormName)
                       .ToListAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> Emails([DataSourceRequest] DataSourceRequest request) 
        {
            var result = new List<AspNetUsers>();

            using (var context = new FMISContext()) 
            {
                result = await context
                    .AspNetUsers
                    .OrderBy(x => x.EmailConfirmed)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }
    }
}
