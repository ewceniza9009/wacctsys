using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticleInventory
    {
        public MstArticleInventory()
        {
            TrnInventory = new HashSet<TrnInventory>();
            TrnSalesInvoiceItem = new HashSet<TrnSalesInvoiceItem>();
            TrnStockOutItem = new HashSet<TrnStockOutItem>();
            TrnStockTransferItem = new HashSet<TrnStockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ArticleId { get; set; }
        [Required]
        [StringLength(50)]
        public string InventoryCode { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.MstArticleInventory))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.MstArticleInventory))]
        public virtual MstBranch Branch { get; set; }
        [InverseProperty("ArticleInventory")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("ItemInventory")]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
        [InverseProperty("ItemInventory")]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItem { get; set; }
        [InverseProperty("ItemInventory")]
        public virtual ICollection<TrnStockTransferItem> TrnStockTransferItem { get; set; }
    }
}
