using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using webfmis.Services;

namespace webfmis.Models
{
    public partial class TrnCollectionLine
    {
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string AccountName { get; set; }
        [NotMapped]
        public string ArticleName { get; set; }
        [NotMapped]
        public string SINumber { get; set; }
        [NotMapped]
        public string PayTypeName { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
