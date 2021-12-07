using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnJournalVoucherLine
    {
        [Key]
        public int Id { get; set; }
        [Column("JVId")]
        public int Jvid { get; set; }
        public int BranchId { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18, 5)")]

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18, 5)")]

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal CreditAmount { get; set; }
        [Column("APRRId")]
        public int? Aprrid { get; set; }
        [Column("ARSIId")]
        public int? Arsiid { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.TrnJournalVoucherLine))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(Aprrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnJournalVoucherLine))]
        public virtual TrnReceivingReceipt Aprr { get; set; }
        [ForeignKey(nameof(Arsiid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnJournalVoucherLine))]
        public virtual TrnSalesInvoice Arsi { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnJournalVoucherLine))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnJournalVoucherLine))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(Jvid))]
        [InverseProperty(nameof(TrnJournalVoucher.TrnJournalVoucherLine))]
        public virtual TrnJournalVoucher Jv { get; set; }
    }
}
