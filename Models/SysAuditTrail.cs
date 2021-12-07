using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class SysAuditTrail
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime AuditDate { get; set; }
        [Required]
        [StringLength(255)]
        public string TableInformation { get; set; }
        [Required]
        [StringLength(255)]
        public string RecordInformation { get; set; }
        [Required]
        [StringLength(255)]
        public string FormInformation { get; set; }
        [Required]
        [StringLength(255)]
        public string ActionInformation { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(MstUser.SysAuditTrail))]
        public virtual MstUser User { get; set; }
    }
}
