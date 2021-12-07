using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstUserForm
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FormId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanLock { get; set; }
        public bool CanUnlock { get; set; }
        public bool CanPrint { get; set; }

        [ForeignKey(nameof(FormId))]
        [InverseProperty(nameof(SysForm.MstUserForm))]
        public virtual SysForm Form { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(MstUser.MstUserForm))]
        public virtual MstUser User { get; set; }
    }
}
