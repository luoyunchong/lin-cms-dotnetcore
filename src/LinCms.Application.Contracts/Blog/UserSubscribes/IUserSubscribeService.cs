using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;

namespace LinCms.Blog.UserSubscribes;

public interface IUserSubscribeService
{
    /// <summary>
    /// 得到某用户的关注的用户Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<long>> GetSubscribeUserIdAsync(long userId);

    /// <summary>
    /// 得到某个用户的关注
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    PagedResultDto<UserSubscribeDto> GetUserSubscribeeeList(UserSubscribeSearchDto searchDto);
    PagedResultDto<UserSubscribeDto> GetUserFansList(UserSubscribeSearchDto searchDto);
    Task CreateAsync(long subscribeUserId);
    Task DeleteAsync(long subscribeUserId);
}