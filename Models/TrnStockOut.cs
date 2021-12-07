using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockOut
    {
        public TrnStockOut()
        {
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnStockOutItem = new HashSet<TrnStockOutItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("OTNumber")]
        [StringLength(50)]
        public string Otnumber { get; set; }
        [Column("OTDate", TypeName = "datetime")]
        public DateTime Otdate { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [Column("ManualOTNumber")]
        [StringLength(50)]
        public string ManualOtnumber { get; set; }
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

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.TrnStockOut))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnStockOut))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnStockOut))]
        public virtual MstBranch Branch { get; set; }
        [InverseProperty("Ot")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("Ot")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("Ot")]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItem { get; set; }
    }
}
