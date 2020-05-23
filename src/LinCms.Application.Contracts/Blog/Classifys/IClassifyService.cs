using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Classifys.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Classifys
{
    public interface IClassifyService:ICrudAppService<ClassifyDto,ClassifyDto,Guid,ClassifySearchDto,CreateUpdateClassifyDto,CreateUpdateClassifyDto>
    {
        List<ClassifyDto> GetListByUserId(long? userId);
        Task UpdateArticleCountAsync(Guid? id, int inCreaseCount);
    }
}