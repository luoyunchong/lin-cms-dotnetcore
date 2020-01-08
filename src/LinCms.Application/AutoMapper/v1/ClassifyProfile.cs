using AutoMapper;
using LinCms.Application.Contracts.v1.Classifys;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
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
