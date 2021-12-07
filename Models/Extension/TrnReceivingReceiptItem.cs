using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;


namespace webfmis.Models
{
    public partial class TrnReceivingReceiptItem
    {
        [NotMapped]
        public string PONumber { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string VatInfo { get; set; }
        [NotMapped]
        public string WTaxInfo { get; set; }
        [NotMapped]
        public string UnitConversionInfo { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }

        [NotMapped]
        public int AccountId => new FMISContext().MstArticle.Find(ItemId)?.AccountId ?? 0;
    }
}
