using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnInventory
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime InventoryDate { get; set; }
        public int ArticleId { get; set; }
        public int ArticleInventoryId { get; set; }
        [Column("RRId")]
        public int? Rrid { get; set; }
        [Column("SIId")]
        public int? Siid { get; set; }
        [Column("INId")]
        public int? Inid { get; set; }
        [Column("OTId")]
        public int? Otid { get; set; }
        [Column("STId")]
        public int? Stid { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal QuantityIn { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal QuantityOut { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnInventory))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(ArticleInventoryId))]
        [InverseProperty(nameof(MstArticleInventory.TrnInventory))]
        public virtual MstArticleInventory ArticleInventory { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnInventory))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(Inid))]
        [InverseProperty(nameof(TrnStockIn.TrnInventory))]
        public virtual TrnStockIn In { get; set; }
        [ForeignKey(nameof(Otid))]
        [InverseProperty(nameof(TrnStockOut.TrnInventory))]
        public virtual TrnStockOut Ot { get; set; }
        [ForeignKey(nameof(Rrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnInventory))]
        public virtual TrnReceivingReceipt Rr { get; set; }
        [ForeignKey(nameof(Siid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnInventory))]
        public virtual TrnSalesInvoice Si { get; set; }
        [ForeignKey(nameof(Stid))]
        [InverseProperty(nameof(TrnStockTransfer.TrnInventory))]
        public virtual TrnStockTransfer St { get; set; }
    }
}
