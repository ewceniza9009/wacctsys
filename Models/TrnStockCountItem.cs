using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockCountItem
    {
        [Key]
        public int Id { get; set; }
        [Column("SCId")]
        public int Scid { get; set; }
        public int ItemId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }

        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(MstArticle.TrnStockCountItem))]
        public virtual MstArticle Item { get; set; }
        [ForeignKey(nameof(Scid))]
        [InverseProperty(nameof(TrnStockCount.TrnStockCountItem))]
        public virtual TrnStockCount Sc { get; set; }
    }
}
