using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnPurchaseOrder
    {
        public TrnPurchaseOrder()
        {
            TrnPurchaseOrderItem = new HashSet<TrnPurchaseOrderItem>();
            TrnReceivingReceiptItem = new HashSet<TrnReceivingReceiptItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("PONumber")]
        [StringLength(50)]
        public string Ponumber { get; set; }
        [Column("PODate", TypeName = "datetime")]
        public DateTime Podate { get; set; }
        public int SupplierId { get; set; }
        public int TermId { get; set; }
        [Required]
        [StringLength(50)]
        public string ManualRequestNumber { get; set; }
        [Required]
        [Column("ManualPONumber")]
        [StringLength(50)]
        public string ManualPonumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateNeeded { get; set; }
        [Required]
        [StringLength(255)]
        public string Remarks { get; set; }
        public bool IsClose { get; set; }
        public int RequestedById { get; set; }
        public int PreparedById { get; set; }
        public int CheckedById { get; set; }
        public int ApprovedById { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnPurchaseOrder))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(MstArticle.TrnPurchaseOrder))]
        public virtual MstArticle Supplier { get; set; }
        [ForeignKey(nameof(TermId))]
        [InverseProperty(nameof(MstTerm.TrnPurchaseOrder))]
        public virtual MstTerm Term { get; set; }
        [InverseProperty("Po")]
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItem { get; set; }
        [InverseProperty("Po")]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItem { get; set; }
    }
}
