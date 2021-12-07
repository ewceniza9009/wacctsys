using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnPurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }
        [Column("POId")]
        public int Poid { get; set; }
        public int ItemId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        public int UnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]

        [Range(0, float.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]

        [Range(0, float.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18, 5)")]

        [Range(0, float.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnPurchaseOrderItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(Poid))]
        [InverseProperty(nameof(TrnPurchaseOrder.TrnPurchaseOrderItem))]
        public virtual TrnPurchaseOrder Po { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnPurchaseOrderItem))]
        public virtual MstUnit Unit { get; set; }
    }
}
