using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstAccountArticleType
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ArticleTypeId { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.MstAccountArticleType))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleTypeId))]
        [InverseProperty(nameof(MstArticleType.MstAccountArticleType))]
        public virtual MstArticleType ArticleType { get; set; }
    }
}
