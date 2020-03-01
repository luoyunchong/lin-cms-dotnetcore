using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Application.Blog.UserSubscribes
{
    public interface IUserSubscribeService
    {
        /// <summary>
        /// 得到某用户的关注的用户Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<long> GetSubscribeUserId(long userId);
    }
}
