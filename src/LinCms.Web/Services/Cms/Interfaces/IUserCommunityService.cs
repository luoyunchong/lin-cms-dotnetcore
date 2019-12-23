using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LinCms.Web.Services.Cms.Interfaces
{
    public interface IUserCommunityService
    {
        long SaveGitHub(ClaimsPrincipal principal, string openId);
    }
}
