using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnJournalVoucher
    {
        public TrnJournalVoucher()
        {
            TrnJournal = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("JVNumber")]
        [StringLength(50)]
        public string Jvnumber { get; set; }
        [Column("JVDate", TypeName = "datetime")]
        public DateTime Jvdate { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [Column("ManualJVNumber")]
        [StringLength(50)]
        public string ManualJvnumber { get; set; }
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

        [InverseProperty("Jv")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("Jv")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
    }
}
