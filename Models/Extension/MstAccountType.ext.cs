using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class MstAccountType
    {
        [NotMapped]
        public string AccountCategoryName => new FMISContext().MstAccountCategory.Find(this.AccountCategoryId)?.AccountCategory;

    }
}
