﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webfmis.Models
{
    public partial class MstAccountArticleType
    {

        [NotMapped]
        public bool IsDeleted { get; set; } = false;
    }
}
