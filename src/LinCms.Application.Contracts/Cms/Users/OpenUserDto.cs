using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Cms.Users;

public class OpenUserDto : EntityDto<long>
{
    public OpenUserDto(string nickname)
    {
        Nickname = nickname;
    }

    public OpenUserDto()
    {
    }

    public string Introduction { get; set; }
    public string Username { get; set; }
    public string Nickname { get; set; }
    public string Avatar { get; set; }

}