using System;
using System.Linq;
using AutoMapper;
using LinCms.Plugins.Poem.Models;
using LinCms.Zero.Domain;

namespace LinCms.Plugins.Poem.AutoMapper
{
    public class PoemProfile : Profile
    {
        public PoemProfile()
        {
            CreateMap<LinPoem, PoemDto>()
                .ForMember(r => r.Content, opts => opts.MapFrom(r => r
                       .Content
                       .Split('|', StringSplitOptions.None)
                       .ToList()
                       .ConvertAll(u => u.Split('/', StringSplitOptions.None).ToList())
               ));

            CreateMap<CreateUpdatePoemDto, LinPoem>();

        }


    }
}
