using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstAccountCashFlow
    {
        public MstAccountCashFlow()
        {
            MstAccount = new HashSet<MstAccount>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string AccountCashFlowCode { get; set; }
        [Required]
        [StringLength(100)]
        public string AccountCashFlow { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("AccountCashFlow")]
        public virtual ICollection<MstAccount> MstAccount { get; set; }
    }
}
