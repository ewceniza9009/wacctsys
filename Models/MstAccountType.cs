using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstAccountType
    {
        public MstAccountType()
        {
            MstAccount = new HashSet<MstAccount>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string AccountTypeCode { get; set; }
        [Required]
        [StringLength(100)]
        public string AccountType { get; set; }
        public int AccountCategoryId { get; set; }
        [Required]
        [StringLength(255)]
        public string SubCategoryDescription { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountCategoryId))]
        [InverseProperty(nameof(MstAccountCategory.MstAccountType))]
        public virtual MstAccountCategory AccountCategory { get; set; }
        [InverseProperty("AccountType")]
        public virtual ICollection<MstAccount> MstAccount { get; set; }
    }
}
