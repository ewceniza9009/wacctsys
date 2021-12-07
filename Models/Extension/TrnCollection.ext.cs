using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;

namespace webfmis.Models
{
    public partial class TrnCollection
    {
        [NotMapped]
        public string BranchName => new FMISContext().MstBranch.FirstOrDefault(x => x.Id == (BranchId == 0 ? TransactionCommon.GetCurrentSettings().BranchId : BranchId))?.Branch;

        [NotMapped]
        public string CustomerName => new FMISContext().MstArticle.FirstOrDefault(x => x.Id == CustomerId)?.Article;

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount => new FMISContext().TrnCollectionLine.Where(x => x.Orid == Id).Sum(x => x.Amount);
    }
}
