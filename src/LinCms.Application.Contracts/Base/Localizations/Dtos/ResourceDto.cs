using LinCms.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LinCms.Application.Contracts.Base.Localizations.Dtos
{
    public class ResourceDto : EntityDto<long>
    {
        [StringLength(50)]
        public string Key { get; set; }
        [StringLength(50)]
        public string Value { get; set; }
        public long CultureId { get; set; }
    }
}
