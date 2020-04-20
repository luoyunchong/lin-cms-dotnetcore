using AutoMapper;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Application.Contracts.Blog.Classifys.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
{
    public class ClassifyProfile : Profile
    {
        public ClassifyProfile()
        {
            CreateMap<CreateUpdateClassifyDto, Classify>();
            CreateMap<Classify, ClassifyDto>();
        }
    }
}
