using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnReceivingReceiptItem
    {
        [Key]
        public int Id { get; set; }
        [Column("RRId")]
        public int Rrid { get; set; }
        [Column("POId")]
        public int Poid { get; set; }
        public int ItemId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        public int UnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }
        [Column("VATId")]
        public int Vatid { get; set; }
        [Column("VATPercentage", TypeName = "decimal(18, 5)")]
        public decimal Vatpercentage { get; set; }
        [Column("VATAmount", TypeName = "decimal(18, 5)")]
        public decimal Vatamount { get; set; }
        [Column("WTAXId")]
        public int Wtaxid { get; set; }
        [Column("WTAXPercentage", TypeName = "decimal(18, 5)")]
        public decimal Wtaxpercentage { get; set; }
        [Column("WTAXAmount", TypeName = "decimal(18, 5)")]
        public decimal Wtaxamount { get; set; }
        public int BranchId { get; set; }
        public int BaseUnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal BaseQuantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal BaseCost { get; set; }

        [ForeignKey(nameof(BaseUnitId))]
        [InverseProperty(nameof(MstUnit.TrnReceivingReceiptItemBaseUnit))]
        public virtual MstUnit BaseUnit { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnReceivingReceiptItem))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnReceivingReceiptItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(Poid))]
        [InverseProperty(nameof(TrnPurchaseOrder.TrnReceivingReceiptItem))]
        public virtual TrnPurchaseOrder Po { get; set; }
        [ForeignKey(nameof(Rrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnReceivingReceiptItem))]
        public virtual TrnReceivingReceipt Rr { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnReceivingReceiptItemUnit))]
        public virtual MstUnit Unit { get; set; }
        [ForeignKey(nameof(Vatid))]
        [InverseProperty(nameof(MstTaxType.TrnReceivingReceiptItemVat))]
        public virtual MstTaxType Vat { get; set; }
        [ForeignKey(nameof(Wtaxid))]
        [InverseProperty(nameof(MstTaxType.TrnReceivingReceiptItemWtax))]
        public virtual MstTaxType Wtax { get; set; }
    }
}
