using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnStockIn
    {
        public TrnStockIn()
        {
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnStockInItem = new HashSet<TrnStockInItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("INNumber")]
        [StringLength(50)]
        public string Innumber { get; set; }
        [Column("INDate", TypeName = "datetime")]
        public DateTime Indate { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [Column("ManualINNumber")]
        [StringLength(50)]
        public string ManualInnumber { get; set; }
        public bool IsProduced { get; set; }
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
        [InverseProperty(nameof(MstAccount.TrnStockIn))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnStockIn))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnStockIn))]
        public virtual MstBranch Branch { get; set; }
        [InverseProperty("In")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("In")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("In")]
        public virtual ICollection<TrnStockInItem> TrnStockInItem { get; set; }
    }
}
