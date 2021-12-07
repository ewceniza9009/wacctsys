using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class RepBankReconciliation
    {
        [Key]
        public int Id { get; set; }

        public int? BranchId { get; set; }
        public int? BankId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateEnd { get; set; }
        public int? PreparedById { get; set; }
        public int? CheckedById { get; set; }
        public int? ApprovedById { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? EndingBalancePerBank { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? TotalDepositInTransit { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? TotalOutstandingWithdrawals { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? AdjustedEndingBalancePerBank { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? EndingBalancePerBook { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? TotalDebit { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? TotalCredit { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? AdjustedEndingBalancePerBooks { get; set; }
    }
}
