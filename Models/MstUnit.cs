using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstUnit
    {
        public MstUnit()
        {
            MstArticle = new HashSet<MstArticle>();
            MstArticleUnit = new HashSet<MstArticleUnit>();
            TrnPurchaseOrderItem = new HashSet<TrnPurchaseOrderItem>();
            TrnReceivingReceiptItemBaseUnit = new HashSet<TrnReceivingReceiptItem>();
            TrnReceivingReceiptItemUnit = new HashSet<TrnReceivingReceiptItem>();
            TrnSalesInvoiceItemBaseUnit = new HashSet<TrnSalesInvoiceItem>();
            TrnSalesInvoiceItemUnit = new HashSet<TrnSalesInvoiceItem>();
            TrnStockInItemBaseUnit = new HashSet<TrnStockInItem>();
            TrnStockInItemUnit = new HashSet<TrnStockInItem>();
            TrnStockOutItemBaseUnit = new HashSet<TrnStockOutItem>();
            TrnStockOutItemUnit = new HashSet<TrnStockOutItem>();
            TrnStockTransferItemBaseUnit = new HashSet<TrnStockTransferItem>();
            TrnStockTransferItemUnit = new HashSet<TrnStockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Unit { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("Unit")]
        public virtual ICollection<MstArticle> MstArticle { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<MstArticleUnit> MstArticleUnit { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItem { get; set; }
        [InverseProperty(nameof(TrnReceivingReceiptItem.BaseUnit))]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItemBaseUnit { get; set; }
        [InverseProperty(nameof(TrnReceivingReceiptItem.Unit))]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItemUnit { get; set; }
        [InverseProperty(nameof(TrnSalesInvoiceItem.BaseUnit))]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItemBaseUnit { get; set; }
        [InverseProperty(nameof(TrnSalesInvoiceItem.Unit))]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItemUnit { get; set; }
        [InverseProperty(nameof(TrnStockInItem.BaseUnit))]
        public virtual ICollection<TrnStockInItem> TrnStockInItemBaseUnit { get; set; }
        [InverseProperty(nameof(TrnStockInItem.Unit))]
        public virtual ICollection<TrnStockInItem> TrnStockInItemUnit { get; set; }
        [InverseProperty(nameof(TrnStockOutItem.BaseUnit))]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItemBaseUnit { get; set; }
        [InverseProperty(nameof(TrnStockOutItem.Unit))]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItemUnit { get; set; }
        [InverseProperty(nameof(TrnStockTransferItem.BaseUnit))]
        public virtual ICollection<TrnStockTransferItem> TrnStockTransferItemBaseUnit { get; set; }
        [InverseProperty(nameof(TrnStockTransferItem.Unit))]
        public virtual ICollection<TrnStockTransferItem> TrnStockTransferItemUnit { get; set; }
    }
}
