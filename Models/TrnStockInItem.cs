using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockInItem
    {
        [Key]
        public int Id { get; set; }
        [Column("INId")]
        public int Inid { get; set; }
        public int ItemId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        public int UnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Cost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int BaseUnitId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseQuantity { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BaseCost { get; set; }

        [ForeignKey(nameof(BaseUnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockInItemBaseUnit))]
        public virtual MstUnit BaseUnit { get; set; }
        [ForeignKey(nameof(Inid))]
        [InverseProperty(nameof(TrnStockIn.TrnStockInItem))]
        public virtual TrnStockIn In { get; set; }
        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnStockInItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.TrnStockInItemUnit))]
        public virtual MstUnit Unit { get; set; }
    }
}
