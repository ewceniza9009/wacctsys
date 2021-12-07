using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Models
{
    public partial class WebMenu
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        [Required]
        [StringLength(50)]
        public string Menu { get; set; }
        [StringLength(50)]
        public string Action { get; set; }
        [StringLength(50)]
        public string Controller { get; set; }
        public int SortId { get; set; }
    }
}
