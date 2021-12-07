using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnCollection
    {
        public TrnCollection()
        {
            TrnCollectionLine = new HashSet<TrnCollectionLine>();
            TrnJournal = new HashSet<TrnJournal>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("ORNumber")]
        [StringLength(50)]
        public string Ornumber { get; set; }
        [Column("ORDate", TypeName = "datetime")]
        public DateTime Ordate { get; set; }
        public int CustomerId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [Column("ManualORNumber")]
        [StringLength(50)]
        public string ManualOrnumber { get; set; }
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
        [InverseProperty(nameof(MstBranch.TrnCollection))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(MstArticle.TrnCollection))]
        public virtual MstArticle Customer { get; set; }
        [InverseProperty("Or")]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLine { get; set; }
        [InverseProperty("Or")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
    }
}
