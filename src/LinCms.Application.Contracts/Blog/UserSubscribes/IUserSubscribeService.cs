using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;

namespace LinCms.Blog.UserSubscribes;

/// <summary>
/// 用户关注（订阅）服务接口
/// </summary>
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

    /// <summary>
    /// 获取某个用户的粉丝
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    PagedResultDto<UserSubscribeDto> GetUserFansList(UserSubscribeSearchDto searchDto);

    /// <summary>
    /// 关注用户
    /// </summary>
    /// <param name="subscribeUserId"></param>
    /// <returns></returns>
    Task CreateAsync(long subscribeUserId);

    /// <summary>
    /// 取消关注
    /// </summary>
    /// <param name="subscribeUserId"></param>
    /// <returns></returns>
    Task DeleteAsync(long subscribeUserId);
}