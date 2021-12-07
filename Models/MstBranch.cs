using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstBranch
    {
        public MstBranch()
        {
            MstArticleInventory = new HashSet<MstArticleInventory>();
            TrnCollection = new HashSet<TrnCollection>();
            TrnCollectionLine = new HashSet<TrnCollectionLine>();
            TrnDisbursement = new HashSet<TrnDisbursement>();
            TrnDisbursementLine = new HashSet<TrnDisbursementLine>();
            TrnInventory = new HashSet<TrnInventory>();
            TrnJournal = new HashSet<TrnJournal>();
            TrnJournalVoucherLine = new HashSet<TrnJournalVoucherLine>();
            TrnPurchaseOrder = new HashSet<TrnPurchaseOrder>();
            TrnReceivingReceipt = new HashSet<TrnReceivingReceipt>();
            TrnReceivingReceiptItem = new HashSet<TrnReceivingReceiptItem>();
            TrnSalesInvoice = new HashSet<TrnSalesInvoice>();
            TrnStockCount = new HashSet<TrnStockCount>();
            TrnStockIn = new HashSet<TrnStockIn>();
            TrnStockOut = new HashSet<TrnStockOut>();
            TrnStockTransferBranch = new HashSet<TrnStockTransfer>();
            TrnStockTransferToBranch = new HashSet<TrnStockTransfer>();
        }

        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        [StringLength(3)]
        public string BranchCode { get; set; }
        [Required]
        [StringLength(100)]
        public string Branch { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactNumber { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(CompanyId))]
        [InverseProperty(nameof(MstCompany.MstBranch))]
        public virtual MstCompany Company { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<MstArticleInventory> MstArticleInventory { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnCollection> TrnCollection { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnCollectionLine> TrnCollectionLine { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnDisbursement> TrnDisbursement { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnInventory> TrnInventory { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnJournal> TrnJournal { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnPurchaseOrder> TrnPurchaseOrder { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnReceivingReceipt> TrnReceivingReceipt { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItem { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnSalesInvoice> TrnSalesInvoice { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnStockCount> TrnStockCount { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnStockIn> TrnStockIn { get; set; }
        [InverseProperty("Branch")]
        public virtual ICollection<TrnStockOut> TrnStockOut { get; set; }
        [InverseProperty(nameof(TrnStockTransfer.Branch))]
        public virtual ICollection<TrnStockTransfer> TrnStockTransferBranch { get; set; }
        [InverseProperty(nameof(TrnStockTransfer.ToBranch))]
        public virtual ICollection<TrnStockTransfer> TrnStockTransferToBranch { get; set; }
    }
}
