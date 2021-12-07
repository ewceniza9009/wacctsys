using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticleGroup
    {
        public MstArticleGroup()
        {
            MstArticle = new HashSet<MstArticle>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string ArticleGroup { get; set; }
        public int ArticleTypeId { get; set; }
        public int AccountId { get; set; }
        public int SalesAccountId { get; set; }
        public int CostAccountId { get; set; }
        public int AssetAccountId { get; set; }
        public int ExpenseAccountId { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(ArticleTypeId))]
        [InverseProperty(nameof(MstArticleType.MstArticleGroup))]
        public virtual MstArticleType ArticleType { get; set; }
        [InverseProperty("ArticleGroup")]
        public virtual ICollection<MstArticle> MstArticle { get; set; }
    }
}
