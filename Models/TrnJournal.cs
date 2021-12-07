using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnJournal
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime JournalDate { get; set; }
        public int BranchId { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal CreditAmount { get; set; }
        [Column("ORId")]
        public int? Orid { get; set; }
        [Column("CVId")]
        public int? Cvid { get; set; }
        [Column("JVId")]
        public int? Jvid { get; set; }
        [Column("RRId")]
        public int? Rrid { get; set; }
        [Column("SIId")]
        public int? Siid { get; set; }
        [Column("INId")]
        public int? Inid { get; set; }
        [Column("OTId")]
        public int? Otid { get; set; }
        [Column("STId")]
        public int? Stid { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentReference { get; set; }
        [Column("APRRId")]
        public int? Aprrid { get; set; }
        [Column("ARSIId")]
        public int? Arsiid { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.TrnJournal))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(Aprrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnJournalAprr))]
        public virtual TrnReceivingReceipt Aprr { get; set; }
        [ForeignKey(nameof(Arsiid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnJournalArsi))]
        public virtual TrnSalesInvoice Arsi { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnJournal))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnJournal))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(Cvid))]
        [InverseProperty(nameof(TrnDisbursement.TrnJournal))]
        public virtual TrnDisbursement Cv { get; set; }
        [ForeignKey(nameof(Inid))]
        [InverseProperty(nameof(TrnStockIn.TrnJournal))]
        public virtual TrnStockIn In { get; set; }
        [ForeignKey(nameof(Jvid))]
        [InverseProperty(nameof(TrnJournalVoucher.TrnJournal))]
        public virtual TrnJournalVoucher Jv { get; set; }
        [ForeignKey(nameof(Orid))]
        [InverseProperty(nameof(TrnCollection.TrnJournal))]
        public virtual TrnCollection Or { get; set; }
        [ForeignKey(nameof(Otid))]
        [InverseProperty(nameof(TrnStockOut.TrnJournal))]
        public virtual TrnStockOut Ot { get; set; }
        [ForeignKey(nameof(Rrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnJournalRr))]
        public virtual TrnReceivingReceipt Rr { get; set; }
        [ForeignKey(nameof(Siid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnJournalSi))]
        public virtual TrnSalesInvoice Si { get; set; }
        [ForeignKey(nameof(Stid))]
        [InverseProperty(nameof(TrnStockTransfer.TrnJournal))]
        public virtual TrnStockTransfer St { get; set; }
    }
}
