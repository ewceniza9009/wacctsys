using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticleUnit
    {
        [Key]
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int UnitId { get; set; }

        [Column(TypeName = "decimal(18, 12)")]
        [Range(0, float.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:N10}")]
        public decimal Multiplier { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.MstArticleUnit))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.MstArticleUnit))]
        public virtual MstUnit Unit { get; set; }
    }
}
