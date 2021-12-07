using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class SysForm
    {
        public SysForm()
        {
            MstUserForm = new HashSet<MstUserForm>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FormName { get; set; }
        [Required]
        [StringLength(255)]
        public string Particulars { get; set; }

        [InverseProperty("Form")]
        public virtual ICollection<MstUserForm> MstUserForm { get; set; }
    }
}
