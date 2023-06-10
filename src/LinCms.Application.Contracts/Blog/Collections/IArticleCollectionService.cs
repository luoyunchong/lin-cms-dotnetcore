using System.Threading.Tasks;

namespace LinCms.Blog.Collections;

public interface IArticleCollectionService:IApplicationService
{
    Task<bool> CreateOrCancelAsync(CreateCancelArticleCollectionDto crDto);
}