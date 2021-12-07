using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class TrnStockCountItem
    {
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
