using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class MstArticleComponent
    {
        [NotMapped]
        public string ComponentManualCode { get; set; }

        [NotMapped]
        public string ComponentUnit { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Cost { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
