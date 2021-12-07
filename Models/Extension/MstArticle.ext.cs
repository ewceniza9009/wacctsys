using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;

namespace webfmis.Models
{
    public partial class MstArticle
    {
        [NotMapped]
        public string ArticleTypeDescription { get; set; }

        [NotMapped]
        public string UnitName => new FMISContext().MstUnit?.Find(UnitId)?.Unit;

        [NotMapped]
        public string TermName => new FMISContext().MstTerm?.Find(TermId)?.Term;


        [NotMapped]
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public int? ItemInventoryId { get; set; }

        [NotMapped]
        public string ArticleImagePath { get; set; }
    }
}
