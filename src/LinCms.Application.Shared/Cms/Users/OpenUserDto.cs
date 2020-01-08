using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Users
{
    public class OpenUserDto : EntityDto
    {
        public string Introduction { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }

    }
}
