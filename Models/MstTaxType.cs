using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstTaxType
    {
        public MstTaxType()
        {
            MstArticleInputTax = new HashSet<MstArticle>();
            MstArticleOutputTax = new HashSet<MstArticle>();
            MstArticleWtaxType = new HashSet<MstArticle>();
            TrnReceivingReceiptItemVat = new HashSet<TrnReceivingReceiptItem>();
            TrnReceivingReceiptItemWtax = new HashSet<TrnReceivingReceiptItem>();
            TrnSalesInvoiceItem = new HashSet<TrnSalesInvoiceItem>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string TaxType { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal TaxRate { get; set; }
        public bool IsInclusive { get; set; }
        public int AccountId { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(MstAccount.MstTaxType))]
        public virtual MstAccount Account { get; set; }
        [InverseProperty(nameof(MstArticle.InputTax))]
        public virtual ICollection<MstArticle> MstArticleInputTax { get; set; }
        [InverseProperty(nameof(MstArticle.OutputTax))]
        public virtual ICollection<MstArticle> MstArticleOutputTax { get; set; }
        [InverseProperty(nameof(MstArticle.WtaxType))]
        public virtual ICollection<MstArticle> MstArticleWtaxType { get; set; }
        [InverseProperty(nameof(TrnReceivingReceiptItem.Vat))]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItemVat { get; set; }
        [InverseProperty(nameof(TrnReceivingReceiptItem.Wtax))]
        public virtual ICollection<TrnReceivingReceiptItem> TrnReceivingReceiptItemWtax { get; set; }
        [InverseProperty("Vat")]
        public virtual ICollection<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
    }
}
