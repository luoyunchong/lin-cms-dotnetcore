using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Dependency;
using LinCms.Web.Domain;

namespace LinCms.Web.Services.Interfaces
{
    public interface IUserService:IScopeDependency
    {
        /// <summary>
        /// 登录授权
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string Authenticate(string username, string password);
    }
}
