using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockTransferItem
    {
        [Key]
        public int Id { get; set; }
        [Column("STId")]
        public int Stid { get; set; }
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
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }
        public int BaseUnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseQuantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseCost { get; set; }

        [ForeignKey(nameof(BaseUnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockTransferItemBaseUnit))]
        public virtual MstUnit BaseUnit { get; set; }
        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnStockTransferItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(ItemInventoryId))]
        [InverseProperty(nameof(MstArticleInventory.TrnStockTransferItem))]
        public virtual MstArticleInventory ItemInventory { get; set; }
        [ForeignKey(nameof(Stid))]
        [InverseProperty(nameof(TrnStockTransfer.TrnStockTransferItem))]
        public virtual TrnStockTransfer St { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockTransferItemUnit))]
        public virtual MstUnit Unit { get; set; }
    }
}
