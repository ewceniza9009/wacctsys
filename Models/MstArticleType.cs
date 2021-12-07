using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticleType
    {
        public MstArticleType()
        {
            MstAccountArticleType = new HashSet<MstAccountArticleType>();
            MstArticle = new HashSet<MstArticle>();
            MstArticleGroup = new HashSet<MstArticleGroup>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string ArticleType { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("ArticleType")]
        public virtual ICollection<MstAccountArticleType> MstAccountArticleType { get; set; }
        [InverseProperty("ArticleType")]
        public virtual ICollection<MstArticle> MstArticle { get; set; }
        [InverseProperty("ArticleType")]
        public virtual ICollection<MstArticleGroup> MstArticleGroup { get; set; }
    }
}
