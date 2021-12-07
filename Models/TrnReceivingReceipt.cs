using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnReceivingReceipt
    {
        public TrnReceivingReceipt()
        {
            TrnDisbursementLine = new HashSet<TrnDisbursementLine>();
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournalAprr = new HashSet<TrnJournal>();
            TrnJournalRr = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
            TrnReceivingReceiptItem = new HashSet<TrnReceivingReceiptItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("RRNumber")]
        [StringLength(50)]
        public string Rrnumber { get; set; }
        [Column("RRDate", TypeName = "datetime")]
        public DateTime Rrdate { get; set; }
        public int SupplierId { get; set; }
        public int TermId { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentReference { get; set; }
        [Required]
        [Column("ManualRRNumber")]
        [StringLength(50)]
        public string ManualRrnumber { get; set; }
        [Required]
        [StringLength(255)]
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        [Column("WTaxAmount", TypeName = "decimal(18, 5)")]
        public decimal WtaxAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal PaidAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal AdjustmentAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal BalanceAmount { get; set; }
        public int ReceivedById { get; set; }
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
        [InverseProperty(nameof(MstBranch.TrnReceivingReceipt))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(MstArticle.TrnReceivingReceipt))]
        public virtual MstArticle Supplier { get; set; }
        [ForeignKey(nameof(TermId))]
        [InverseProperty(nameof(MstTerm.TrnReceivingReceipt))]
        public virtual MstTerm Term { get; set; }
        [InverseProperty("Rr")]
        public virtual ICollection<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        [InverseProperty("Rr")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty(nameof(TrnJournal.Aprr))]
        public virtual ICollection<TrnJournal> TrnJournalAprr { get; set; }
        [InverseProperty(nameof(TrnJournal.Rr))]
        public virtual ICollection<TrnJournal> TrnJournalRr { get; set; }
        [InverseProperty("Aprr")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        [InverseProperty("Rr")]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItem { get; set; }
    }
}
