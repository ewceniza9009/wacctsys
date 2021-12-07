using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticleComponent
    {
        [Key]
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int ComponentArticleId { get; set; }
        [Column(TypeName = "decimal(18, 8)")]
        [Range(0, float.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.MstArticleComponentArticle))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(ComponentArticleId))]
        [InverseProperty(nameof(MstArticle.MstArticleComponentComponentArticle))]
        public virtual MstArticle ComponentArticle { get; set; }
    }
}
