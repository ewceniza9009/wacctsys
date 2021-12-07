using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockOutItem
    {
        [Key]
        public int Id { get; set; }
        [Column("OTId")]
        public int Otid { get; set; }
        public int ExpenseAccountId { get; set; }
        public int ItemId { get; set; }
        public int ItemInventoryId { get; set; }
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
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        public int BaseUnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseQuantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseCost { get; set; }

        [ForeignKey(nameof(BaseUnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockOutItemBaseUnit))]
        public virtual MstUnit BaseUnit { get; set; }
        [ForeignKey(nameof(ExpenseAccountId))]
        [InverseProperty(nameof(MstAccount.TrnStockOutItem))]
        public virtual MstAccount ExpenseAccount { get; set; }
        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnStockOutItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(ItemInventoryId))]
        [InverseProperty(nameof(MstArticleInventory.TrnStockOutItem))]
        public virtual MstArticleInventory ItemInventory { get; set; }
        [ForeignKey(nameof(Otid))]
        [InverseProperty(nameof(TrnStockOut.TrnStockOutItem))]
        public virtual TrnStockOut Ot { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockOutItemUnit))]
        public virtual MstUnit Unit { get; set; }
    }
}
