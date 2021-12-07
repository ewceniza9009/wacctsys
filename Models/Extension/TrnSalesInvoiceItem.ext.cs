using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;

namespace webfmis.Models
{
    public partial class TrnSalesInvoiceItem
    {
        [NotMapped]
        public string ItemInventoryCode { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string VatInfo { get; set; }
        [NotMapped]
        public string WTaxInfo { get; set; }
        [NotMapped]
        public string UnitConversionInfo { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }

        [NotMapped]
        public MstArticle ArticleSalesAccount => new FMISContext().MstArticle.Find(ItemId);

        [NotMapped]
        public string ArticleImagePath { get; set; }
    }
}
