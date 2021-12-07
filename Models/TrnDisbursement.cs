using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnDisbursement
    {
        public TrnDisbursement()
        {
            TrnDisbursementLine = new HashSet<TrnDisbursementLine>();
            TrnJournal = new HashSet<TrnJournal>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("CVNumber")]
        [StringLength(50)]
        public string Cvnumber { get; set; }
        [Column("CVDate", TypeName = "datetime")]
        public DateTime Cvdate { get; set; }
        public int SupplierId { get; set; }
        [Required]
        [StringLength(255)]
        public string Payee { get; set; }
        public int PayTypeId { get; set; }
        public int BankId { get; set; }
        [Required]
        [Column("ManualCVNumber")]
        [StringLength(50)]
        public string ManualCvnumber { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [StringLength(50)]
        public string CheckNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CheckDate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        public bool IsCrossCheck { get; set; }
        public bool IsClear { get; set; }
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

        [ForeignKey(nameof(BankId))]
        [InverseProperty(nameof(MstArticle.TrnDisbursementBank))]
        public virtual MstArticle Bank { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnDisbursement))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(PayTypeId))]
        [InverseProperty(nameof(MstPayType.TrnDisbursement))]
        public virtual MstPayType PayType { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(MstArticle.TrnDisbursementSupplier))]
        public virtual MstArticle Supplier { get; set; }
        [InverseProperty("Cv")]
        public virtual ICollection<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        [InverseProperty("Cv")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
    }
}
