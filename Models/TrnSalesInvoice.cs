using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnSalesInvoice
    {
        public TrnSalesInvoice()
        {
            TrnCollectionLine = new HashSet<TrnCollectionLine>();
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournalArsi = new HashSet<TrnJournal>();
            TrnJournalSi = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
            TrnSalesInvoiceItem = new HashSet<TrnSalesInvoiceItem>();
        }

        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [Column("SINumber")]
        [StringLength(50)]
        public string Sinumber { get; set; }
        [Column("SIDate", TypeName = "datetime")]
        public DateTime Sidate { get; set; }
        public int CustomerId { get; set; }
        public int TermId { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentReference { get; set; }
        [Required]
        [Column("ManualSINumber")]
        [StringLength(50)]
        public string ManualSinumber { get; set; }
        [Required]
        [StringLength(255)]
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal PaidAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal AdjustmentAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal BalanceAmount { get; set; }
        public int SoldById { get; set; }
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
        [InverseProperty(nameof(MstBranch.TrnSalesInvoice))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(MstArticle.TrnSalesInvoice))]
        public virtual MstArticle Customer { get; set; }
        [ForeignKey(nameof(TermId))]
        [InverseProperty(nameof(MstTerm.TrnSalesInvoice))]
        public virtual MstTerm Term { get; set; }
        [InverseProperty("Si")]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLine { get; set; }
        [InverseProperty("Si")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty(nameof(TrnJournal.Arsi))]
        public virtual ICollection<TrnJournal> TrnJournalArsi { get; set; }
        [InverseProperty(nameof(TrnJournal.Si))]
        public virtual ICollection<TrnJournal> TrnJournalSi { get; set; }
        [InverseProperty("Arsi")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        [InverseProperty("Si")]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
    }
}
