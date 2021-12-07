using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class MstCompany
    {
        public MstCompany()
        {
            MstBranch = new HashSet<MstBranch>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]

        [Display(Name = "Company Name")]
        public string Company { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Contact #")]
        public string ContactNumber { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedDateTime { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<MstBranch> MstBranch { get; set; }
    }
}
