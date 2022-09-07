using System;
using System.Collections.Generic;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Cms.Groups;

namespace LinCms.Cms.Users;

public class UserDto : EntityDto<long>
{
    public string Username { get; set; }
    public string Nickname { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
    public int Admin { get; set; } = 1;
    public int Active { get; set; }
    public List<GroupDto> Groups { get; set; }
    public DateTime CreateTime { get; set; }
}