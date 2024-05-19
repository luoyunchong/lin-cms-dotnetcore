using JetBrains.Annotations;
using LinCms.Data;

namespace LinCms.Cms.Admins;

public class UserSearchDto : PageDto
{
    public int? GroupId { get; set; }
    
    [CanBeNull] public string Email { get; set; }
    [CanBeNull] public string Nickname { get; set; }
    [CanBeNull] public string Username { get; set; }
}