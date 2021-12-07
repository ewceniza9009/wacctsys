using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webfmis.Models
{
    public partial class MstUnit
    {
        [NotMapped]
        public List<int> Articles { get; set; }
    }
}
