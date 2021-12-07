using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstPayType
    {
        public MstPayType()
        {
            TrnCollectionLine = new HashSet<TrnCollectionLine>();
            TrnDisbursement = new HashSet<TrnDisbursement>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string PayType { get; set; }
        public int AccountId { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.MstPayType))]
        public virtual MstAccount Account { get; set; }
        [InverseProperty("PayType")]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLine { get; set; }
        [InverseProperty("PayType")]
        public virtual ICollection<TrnDisbursement> TrnDisbursement { get; set; }
    }
}
