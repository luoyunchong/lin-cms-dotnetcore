using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinCms.Blog.Classifys;

public interface IClassifyService : ICrudAppService<ClassifyDto, ClassifyDto, Guid, ClassifySearchDto, CreateUpdateClassifyDto, CreateUpdateClassifyDto>
{
    List<ClassifyDto> GetListByUserId(long? userId);
    Task UpdateArticleCountAsync(Guid? id, int inCreaseCount);
}