using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class MstArticleUnit
    {
        [NotMapped]
        public string BaseUnitName { get; set; }

        [NotMapped]
        [Display(Name = "=")]
        public string EqualSign { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
