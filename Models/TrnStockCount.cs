using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockCount
    {
        public TrnStockCount()
        {
            TrnStockCountItem = new HashSet<TrnStockCountItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("SCNumber")]
        [StringLength(50)]
        public string Scnumber { get; set; }
        [Column("SCDate", TypeName = "datetime")]
        public DateTime Scdate { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
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
        [InverseProperty(nameof(MstBranch.TrnStockCount))]
        public virtual MstBranch Branch { get; set; }
        [InverseProperty("Sc")]
        public virtual ICollection<TrnStockCountItem> TrnStockCountItem { get; set; }
    }
}
