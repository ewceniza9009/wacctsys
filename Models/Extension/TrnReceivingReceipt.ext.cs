using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;

namespace webfmis.Models
{
    public partial class TrnReceivingReceipt
    {
        [NotMapped]
        public string BranchName => new FMISContext().MstBranch.FirstOrDefault(x => x.Id == (BranchId == 0 ? TransactionCommon.GetCurrentSettings().BranchId : BranchId))?.Branch;

        [NotMapped]
        public string SupplierName => new FMISContext().MstArticle.FirstOrDefault(x => x.Id == SupplierId)?.Article;

        [NotMapped]
        public int SelectPOId { get; set; }
    }
}
