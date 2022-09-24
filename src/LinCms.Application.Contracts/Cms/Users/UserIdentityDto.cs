using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Cms.Users;

public class UserIdentityDto : EntityDto<Guid>
{
    public string IdentityType { get; set; }

    public string Identifier { get; set; }

    public string ExtraProperties { get; set; }

    public DateTime CreateTime { get; set; }
}