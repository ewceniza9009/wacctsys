using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class TrnStockInItem
    {
        [NotMapped]
        public int AccountId => new FMISContext().MstArticle.Find(ItemId).AccountId;

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
