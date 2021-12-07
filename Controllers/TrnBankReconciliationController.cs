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
using webfmis.Utils;

namespace webfmis.Controllers
{
    [Authorize]
    public class TrnBankReconciliationController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRights"] = Library.Security.GetFormRights("TrnBankReconciliation");

            return View(new DTO.BankReconciliation());
        }

        [HttpPost]
        public async Task<IActionResult> Print(RepBankReconciliation bankRecon)
        {
            var mappingProfile = new MappingProfile<RepBankReconciliation, RepBankReconciliation>();
            var entity = new RepBankReconciliation();

            using (var context = new FMISContextRepBankReconciliation())
            {
                entity = mappingProfile.mapper.Map<RepBankReconciliation>(bankRecon);
                entity.BranchId = External.ReadSettings().Id;

                await context.RepBankReconciliation.AddAsync(entity);

                await context.SaveChangesAsync();
            }

            return Json(entity.Id);
        }

        public IActionResult DepositGrid([DataSourceRequest] DataSourceRequest request, Queries.Deposit data) 
        {
            var result = new List<DTO.Deposit>();

            if (data.BankId != 0) 
            {
                var query = new Queries.Deposit();

                query.BankId = data.BankId;
                query.DateStart = data.DateStart;
                query.DateEnd = data.DateEnd;

                result = query.Result();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult WithdrawalGrid([DataSourceRequest] DataSourceRequest request, Queries.Withdrawal data)
        {
            var result = new List<DTO.Withdrawal>();

            if (data.BankId != 0)
            {
                var query = new Queries.Withdrawal();

                query.BankId = data.BankId;
                query.DateStart = data.DateStart;
                query.DateEnd = data.DateEnd;

                result = query.Result();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult AdjustmentGrid([DataSourceRequest] DataSourceRequest request, Queries.Adjustment data)
        {
            var result = new List<DTO.Adjustment>();

            if (data.BankId != 0)
            {
                var query = new Queries.Adjustment();

                query.BankId = data.BankId;
                query.DateStart = data.DateStart;
                query.DateEnd = data.DateEnd;

                result = query.Result();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Users([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstUser>();

            using (var context = new FMISContext())
            {
                result = await context.MstUser
                    .OrderBy(x => x.FullName)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> Banks([DataSourceRequest] DataSourceRequest request)
        {
            var result = new List<MstArticle>();

            using (var context = new FMISContext())
            {
                result = await context.MstArticle
                    .OrderBy(x => x.Account)
                    .Where(x => x.ArticleTypeId == 5)
                    .ToListAsync();
            }

            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult GetBankReconciliationValues(int BankId, DateTime dateStart, DateTime dateEnd, decimal endingBalanceAsPerBank) 
        {
            var depositInTransit = 0m;
            var outstandingWithdrawal = 0m;
            var bankAdjustedBalance = 0m;
            var endingBalancePerBook = 0m;
            var debit = 0m;
            var credit = 0m;
            var adjustedEndingBalanceBook = 0m;

            if (BankId != 0)
            {
                var deposit = new Queries.Deposit();
                var withdrawal = new Queries.Withdrawal();
                var adjustment = new Queries.Adjustment();
                var endingBalanceBook = new Queries.EndBalanceBook();

                deposit.BankId = BankId;
                deposit.DateStart = dateStart;
                deposit.DateEnd = dateEnd;

                withdrawal.BankId = BankId;
                withdrawal.DateStart = dateStart;
                withdrawal.DateEnd = dateEnd;

                adjustment.BankId = BankId;
                adjustment.DateStart = dateStart;
                adjustment.DateEnd = dateEnd;

                endingBalanceBook.DateStart = dateStart;
                endingBalanceBook.BankId = BankId;

                depositInTransit = deposit.Result().Sum(x => x.InTransit);
                outstandingWithdrawal = withdrawal.Result().Sum(x => x.Outstanding);

                bankAdjustedBalance = endingBalanceAsPerBank + depositInTransit - outstandingWithdrawal;

                endingBalancePerBook = endingBalanceBook.Result().Sum(x => x.BookBalance);
                debit = adjustment.Result().Sum(x => x.DebitAmount);
                credit = adjustment.Result().Sum(x => x.CreditAmount);

                adjustedEndingBalanceBook = endingBalancePerBook + debit - credit;
            }

            return Json(new { 
                DepositInTransit = depositInTransit, 
                OutstandingWithdrawal = outstandingWithdrawal, 
                BankAdjustedBalance = bankAdjustedBalance,
                EndingBalancePerBook = endingBalancePerBook,
                Debit = debit,
                Credit = credit,
                AdjustedEndingBalanceBook = adjustedEndingBalanceBook
            });
        }
    }
}
