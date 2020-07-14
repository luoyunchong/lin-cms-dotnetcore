using LinCms.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LinCms.Application.Contracts.Base.Localizations.Dtos
{
    public class CultureDto : EntityDto<long>
    {
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string DisplayName { get; set; }
    }
}
