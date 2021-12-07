using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnDisbursementLine
    {
        [Key]
        public int Id { get; set; }
        [Column("CVId")]
        public int Cvid { get; set; }
        public int BranchId { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Column("RRId")]
        public int? Rrid { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.TrnDisbursementLine))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnDisbursementLine))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnDisbursementLine))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(Cvid))]
        [InverseProperty(nameof(TrnDisbursement.TrnDisbursementLine))]
        public virtual TrnDisbursement Cv { get; set; }
        [ForeignKey(nameof(Rrid))]
        [InverseProperty(nameof(TrnReceivingReceipt.TrnDisbursementLine))]
        public virtual TrnReceivingReceipt Rr { get; set; }
    }
}
