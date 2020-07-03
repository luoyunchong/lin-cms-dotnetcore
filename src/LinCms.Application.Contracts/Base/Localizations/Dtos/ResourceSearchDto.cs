using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Base.Localizations.Dtos
{
    public class ResourceSearchDto : PageDto
    {
        public long CultureId { get; set; }
    }
}
