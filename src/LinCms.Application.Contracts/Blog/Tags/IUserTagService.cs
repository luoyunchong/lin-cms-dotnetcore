using System;
using System.Threading.Tasks;

namespace LinCms.Blog.Tags;

public interface IUserTagService
{
    /// <summary>
    /// 用户关注标签
    /// </summary>
    /// <param name="tagId"></param>
    Task CreateUserTagAsync(Guid tagId);

    /// <summary>
    /// 当前用户取消关注标签
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    Task DeleteUserTagAsync(Guid tagId);
}