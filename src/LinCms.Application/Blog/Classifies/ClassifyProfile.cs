using AutoMapper;
using LinCms.Blog.Classifys;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Classifies
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
