using AutoMapper;
using LinCms.Entities.Base;

namespace LinCms.Base.Docs
{
    public class DocProfile : Profile
    {
        public DocProfile()
        {
            CreateMap<Doc, DocDto>();
            CreateMap<CreateUpdateDocDto, Doc>();
        }
    }
}
