using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnSalesInvoiceItem
    {
        [Key]
        public int Id { get; set; }
        [Column("SIId")]
        public int Siid { get; set; }
        public int ItemId { get; set; }
        public int? ItemInventoryId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        public int UnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Price { get; set; }
        public int DiscountId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DiscountRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal NetPrice { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }
        [Column("VATId")]
        public int Vatid { get; set; }
        [Column("VATPercentage", TypeName = "decimal(18, 5)")]
        public decimal Vatpercentage { get; set; }
        [Column("VATAmount", TypeName = "decimal(18, 5)")]
        public decimal Vatamount { get; set; }
        public int BaseUnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal BaseQuantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal BasePrice { get; set; }

        [ForeignKey(nameof(BaseUnitId))]
        [InverseProperty(nameof(MstUnit.TrnSalesInvoiceItemBaseUnit))]
        public virtual MstUnit BaseUnit { get; set; }
        [ForeignKey(nameof(DiscountId))]
        [InverseProperty(nameof(MstDiscount.TrnSalesInvoiceItem))]
        public virtual MstDiscount Discount { get; set; }
        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnSalesInvoiceItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(ItemInventoryId))]
        [InverseProperty(nameof(MstArticleInventory.TrnSalesInvoiceItem))]
        public virtual MstArticleInventory ItemInventory { get; set; }
        [ForeignKey(nameof(Siid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnSalesInvoiceItem))]
        public virtual TrnSalesInvoice Si { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnSalesInvoiceItemUnit))]
        public virtual MstUnit Unit { get; set; }
        [ForeignKey(nameof(Vatid))]
        [InverseProperty(nameof(MstTaxType.TrnSalesInvoiceItem))]
        public virtual MstTaxType Vat { get; set; }
    }
}
