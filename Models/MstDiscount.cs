using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstDiscount
    {
        public MstDiscount()
        {
            TrnSalesInvoiceItem = new HashSet<TrnSalesInvoiceItem>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Discount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal DiscountRate { get; set; }
        public bool IsInclusive { get; set; }
        public int AccountId { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.MstDiscount))]
        public virtual MstAccount Account { get; set; }
        [InverseProperty("Discount")]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
    }
}
