using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class TrnStockOutItem
    {
        [NotMapped]
        public string InventoryCode { get; set; }
        [NotMapped]
        public string ExpenseAccountName { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
