using LinCms.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Application.Contracts.Cms.Users.Dtos
{
    public class UserIdentityDto : EntityDto<Guid>
    {
        public string IdentityType { get; set; }

        public string Identifier { get; set; }

        public string ExtraProperties { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
