using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstTerm
    {
        public MstTerm()
        {
            MstArticle = new HashSet<MstArticle>();
            TrnPurchaseOrder = new HashSet<TrnPurchaseOrder>();
            TrnReceivingReceipt = new HashSet<TrnReceivingReceipt>();
            TrnSalesInvoice = new HashSet<TrnSalesInvoice>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Term { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal NumberOfDays { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("Term")]
        public virtual ICollection<MstArticle> MstArticle { get; set; }
        [InverseProperty("Term")]
        public virtual ICollection<TrnPurchaseOrder> TrnPurchaseOrder { get; set; }
        [InverseProperty("Term")]
        public virtual ICollection<TrnReceivingReceipt> TrnReceivingReceipt { get; set; }
        [InverseProperty("Term")]
        public virtual ICollection<TrnSalesInvoice> TrnSalesInvoice { get; set; }
    }
}
