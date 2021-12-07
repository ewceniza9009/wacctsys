using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstArticle
    {
        public MstArticle()
        {
            MstArticleComponentArticle = new HashSet<MstArticleComponent>();
            MstArticleComponentComponentArticle = new HashSet<MstArticleComponent>();
            MstArticleInventory = new HashSet<MstArticleInventory>();
            MstArticleUnit = new HashSet<MstArticleUnit>();
            TrnCollection = new HashSet<TrnCollection>();
            TrnCollectionLineArticle = new HashSet<TrnCollectionLine>();
            TrnCollectionLineDepositoryBank = new HashSet<TrnCollectionLine>();
            TrnDisbursementBank = new HashSet<TrnDisbursement>();
            TrnDisbursementLine = new HashSet<TrnDisbursementLine>();
            TrnDisbursementSupplier = new HashSet<TrnDisbursement>();
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
            TrnPurchaseOrder = new HashSet<TrnPurchaseOrder>();
            TrnPurchaseOrderItem = new HashSet<TrnPurchaseOrderItem>();
            TrnReceivingReceipt = new HashSet<TrnReceivingReceipt>();
            TrnReceivingReceiptItem = new HashSet<TrnReceivingReceiptItem>();
            TrnSalesInvoice = new HashSet<TrnSalesInvoice>();
            TrnSalesInvoiceItem = new HashSet<TrnSalesInvoiceItem>();
            TrnStockCountItem = new HashSet<TrnStockCountItem>();
            TrnStockIn = new HashSet<TrnStockIn>();
            TrnStockInItem = new HashSet<TrnStockInItem>();
            TrnStockOut = new HashSet<TrnStockOut>();
            TrnStockOutItem = new HashSet<TrnStockOutItem>();
            TrnStockTransferItem = new HashSet<TrnStockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string ArticleCode { get; set; }
        [StringLength(50)]
        public string ManualArticleCode { get; set; }
        [Required]
        [StringLength(255)]
        public string Article { get; set; }
        [Required]
        [StringLength(255)]
        public string Category { get; set; }
        public int ArticleTypeId { get; set; }
        public int? ArticleGroupId { get; set; }
        public int AccountId { get; set; }
        public int SalesAccountId { get; set; }
        public int CostAccountId { get; set; }
        public int AssetAccountId { get; set; }
        public int ExpenseAccountId { get; set; }
        public int UnitId { get; set; }
        public int OutputTaxId { get; set; }
        public int InputTaxId { get; set; }
        [Column("WTaxTypeId")]
        public int WtaxTypeId { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? Cost { get; set; }
        public bool IsInventory { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        public int TermId { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; }
        [Required]
        [StringLength(100)]
        public string TaxNumber { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal CreditLimit { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAcquired { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal UsefulLife { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal SalvageValue { get; set; }
        [StringLength(50)]
        public string ManualArticleOldCode { get; set; }
        [StringLength(500)]
        public string ArticleImage { get; set; }
        [StringLength(500)]
        public string OldArticleImage { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.MstArticleAccount))]
        public virtual MstAccount Account { get; set; }
        [ForeignKey(nameof(ArticleGroupId))]
        [InverseProperty(nameof(MstArticleGroup.MstArticle))]
        public virtual MstArticleGroup ArticleGroup { get; set; }
        [ForeignKey(nameof(ArticleTypeId))]
        [InverseProperty(nameof(MstArticleType.MstArticle))]
        public virtual MstArticleType ArticleType { get; set; }
        [ForeignKey(nameof(AssetAccountId))]
        [InverseProperty(nameof(MstAccount.MstArticleAssetAccount))]
        public virtual MstAccount AssetAccount { get; set; }
        [ForeignKey(nameof(CostAccountId))]
        [InverseProperty(nameof(MstAccount.MstArticleCostAccount))]
        public virtual MstAccount CostAccount { get; set; }
        [ForeignKey(nameof(InputTaxId))]
        [InverseProperty(nameof(MstTaxType.MstArticleInputTax))]
        public virtual MstTaxType InputTax { get; set; }
        [ForeignKey(nameof(OutputTaxId))]
        [InverseProperty(nameof(MstTaxType.MstArticleOutputTax))]
        public virtual MstTaxType OutputTax { get; set; }
        [ForeignKey(nameof(SalesAccountId))]
        [InverseProperty(nameof(MstAccount.MstArticleSalesAccount))]
        public virtual MstAccount SalesAccount { get; set; }
        [ForeignKey(nameof(TermId))]
        [InverseProperty(nameof(MstTerm.MstArticle))]
        public virtual MstTerm Term { get; set; }
        [ForeignKey(nameof(UnitId))]
        [InverseProperty(nameof(MstUnit.MstArticle))]
        public virtual MstUnit Unit { get; set; }
        [ForeignKey(nameof(WtaxTypeId))]
        [InverseProperty(nameof(MstTaxType.MstArticleWtaxType))]
        public virtual MstTaxType WtaxType { get; set; }
        [InverseProperty(nameof(MstArticleComponent.Article))]
        public virtual ICollection<MstArticleComponent> MstArticleComponentArticle { get; set; }
        [InverseProperty(nameof(MstArticleComponent.ComponentArticle))]
        public virtual ICollection<MstArticleComponent> MstArticleComponentComponentArticle { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<MstArticleInventory> MstArticleInventory { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<MstArticleUnit> MstArticleUnit { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<TrnCollection> TrnCollection { get; set; }
        [InverseProperty(nameof(TrnCollectionLine.Article))]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLineArticle { get; set; }
        [InverseProperty(nameof(TrnCollectionLine.DepositoryBank))]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLineDepositoryBank { get; set; }
        [InverseProperty(nameof(TrnDisbursement.Bank))]
        public virtual ICollection<TrnDisbursement> TrnDisbursementBank { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        [InverseProperty(nameof(TrnDisbursement.Supplier))]
        public virtual ICollection<TrnDisbursement> TrnDisbursementSupplier { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        [InverseProperty("Supplier")]
        public virtual ICollection<TrnPurchaseOrder> TrnPurchaseOrder { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnPurchaseOrderItem> TrnPurchaseOrderItem { get; set; }
        [InverseProperty("Supplier")]
        public virtual ICollection<TrnReceivingReceipt> TrnReceivingReceipt { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItem { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<TrnSalesInvoice> TrnSalesInvoice { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnStockCountItem> TrnStockCountItem { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnStockIn> TrnStockIn { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnStockInItem> TrnStockInItem { get; set; }
        [InverseProperty("Article")]
        public virtual ICollection<TrnStockOut> TrnStockOut { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnStockOutItem> TrnStockOutItem { get; set; }
        [InverseProperty("Item")]
        public virtual ICollection<TrnStockTransferItem> TrnStockTransferItem { get; set; }
    }
}
