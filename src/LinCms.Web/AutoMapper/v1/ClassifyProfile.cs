using AutoMapper;
using LinCms.Web.Models.v1.Classifys;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
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
