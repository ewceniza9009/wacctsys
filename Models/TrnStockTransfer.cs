using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockTransfer
    {
        public TrnStockTransfer()
        {
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnStockTransferItem = new HashSet<TrnStockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("STNumber")]
        [StringLength(50)]
        public string Stnumber { get; set; }
        [Column("STDate", TypeName = "datetime")]
        public DateTime Stdate { get; set; }
        public int ToBranchId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [Column("ManualSTNumber")]
        [StringLength(255)]
        public string ManualStnumber { get; set; }
        public int PreparedById { get; set; }
        public int CheckedById { get; set; }
        public int ApprovedById { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnStockTransferBranch))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(ToBranchId))]
        [InverseProperty(nameof(MstBranch.TrnStockTransferToBranch))]
        public virtual MstBranch ToBranch { get; set; }
        [InverseProperty("St")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("St")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("St")]
        public virtual ICollection<TrnStockTransferItem> TrnStockTransferItem { get; set; }
    }
}
