using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class TrnCollectionLine
    {
        [Key]
        public int Id { get; set; }
        [Column("ORId")]
        public int Orid { get; set; }
        public int BranchId { get; set; }
        public int AccountId { get; set; }
        public int ArticleId { get; set; }
        [Column("SIId")]
        public int? Siid { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }
        public int PayTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string CheckNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CheckDate { get; set; }
        [Required]
        [StringLength(100)]
        public string CheckBank { get; set; }
        public int? DepositoryBankId { get; set; }
        public bool IsClear { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.TrnCollectionLine))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(MstArticle.TrnCollectionLineArticle))]
        public virtual MstArticle Article { get; set; }
        [ForeignKey(nameof(BranchId))]
        [InverseProperty(nameof(MstBranch.TrnCollectionLine))]
        public virtual MstBranch Branch { get; set; }
        [ForeignKey(nameof(DepositoryBankId))]
        [InverseProperty(nameof(MstArticle.TrnCollectionLineDepositoryBank))]
        public virtual MstArticle DepositoryBank { get; set; }
        [ForeignKey(nameof(Orid))]
        [InverseProperty(nameof(TrnCollection.TrnCollectionLine))]
        public virtual TrnCollection Or { get; set; }
        [ForeignKey(nameof(PayTypeId))]
        [InverseProperty(nameof(MstPayType.TrnCollectionLine))]
        public virtual MstPayType PayType { get; set; }
        [ForeignKey(nameof(Siid))]
        [InverseProperty(nameof(TrnSalesInvoice.TrnCollectionLine))]
        public virtual TrnSalesInvoice Si { get; set; }
    }
}
