using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstUser
    {
        public MstUser()
        {
            MstUserForm = new HashSet<MstUserForm>();
            SysAuditTrail = new HashSet<SysAuditTrail>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        public string ASPUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<MstUserForm> MstUserForm { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<SysAuditTrail> SysAuditTrail { get; set; }
    }
}
