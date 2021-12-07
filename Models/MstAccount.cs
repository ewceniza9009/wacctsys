using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstAccount
    {
        public MstAccount()
        {
            MstAccountArticleType = new HashSet<MstAccountArticleType>();
            MstArticleAccount = new HashSet<MstArticle>();
            MstArticleAssetAccount = new HashSet<MstArticle>();
            MstArticleCostAccount = new HashSet<MstArticle>();
            MstArticleSalesAccount = new HashSet<MstArticle>();
            MstDiscount = new HashSet<MstDiscount>();
            MstPayType = new HashSet<MstPayType>();
            MstTaxType = new HashSet<MstTaxType>();
            TrnCollectionLine = new HashSet<TrnCollectionLine>();
            TrnDisbursementLine = new HashSet<TrnDisbursementLine>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
            TrnStockIn = new HashSet<TrnStockIn>();
            TrnStockOut = new HashSet<TrnStockOut>();
            TrnStockOutItem = new HashSet<TrnStockOutItem>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Required]
        [StringLength(100)]
        public string Account { get; set; }

        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }

        [Display(Name = "Cash Flow")]
        public int? AccountCashFlowId { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountCashFlowId))]
        [InverseProperty(nameof(MstAccountCashFlow.MstAccount))]
        public virtual MstAccountCashFlow AccountCashFlow { get; set; }
        [ForeignKey(nameof(AccountTypeId))]
        [InverseProperty(nameof(MstAccountType.MstAccount))]
        public virtual MstAccountType AccountType { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<MstAccountArticleType> MstAccountArticleType { get; set; }
        [InverseProperty(nameof(MstArticle.Account))]
        public virtual ICollection<MstArticle> MstArticleAccount { get; set; }
        [InverseProperty(nameof(MstArticle.AssetAccount))]
        public virtual ICollection<MstArticle> MstArticleAssetAccount { get; set; }
        [InverseProperty(nameof(MstArticle.CostAccount))]
        public virtual ICollection<MstArticle> MstArticleCostAccount { get; set; }
        [InverseProperty(nameof(MstArticle.SalesAccount))]
        public virtual ICollection<MstArticle> MstArticleSalesAccount { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<MstDiscount> MstDiscount { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<MstPayType> MstPayType { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<MstTaxType> MstTaxType { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLine { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnStockIn> TrnStockIn { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<TrnStockOut> TrnStockOut { get; set; }
        [InverseProperty("ExpenseAccount")]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItem { get; set; }
    }
}
