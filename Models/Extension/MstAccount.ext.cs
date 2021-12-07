using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class MstAccount
    {
        [NotMapped]
        public string AccountTypeName => new FMISContext().MstAccountType.Find(this.AccountTypeId)?.AccountType;
    }
}
