using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;

namespace webfmis.Models
{
    public partial class TrnJournal
    {
        [NotMapped]
        public string BranchName => new FMISContext().MstBranch.FirstOrDefault(x => x.Id == (BranchId == 0 ? TransactionCommon.GetCurrentSettings().BranchId : BranchId))?.Branch;
        [NotMapped]
        public string Code => new FMISContext().MstAccount.FirstOrDefault(x => x.Id == AccountId)?.AccountCode;
        [NotMapped]
        public string AccountName => new FMISContext().MstAccount.FirstOrDefault(x => x.Id == AccountId)?.Account;
        [NotMapped]
        public string ArticleName => new FMISContext().MstArticle.FirstOrDefault(x => x.Id == ArticleId)?.Article;
    }
}
