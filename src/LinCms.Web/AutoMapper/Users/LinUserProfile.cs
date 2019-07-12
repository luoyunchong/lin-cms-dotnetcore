using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Domain;
using LinCms.Web.Models.Users;

namespace LinCms.Web.AutoMapper.Users
{
    public class LinUserProfile : Profile
    {
        public LinUserProfile()
        {
            CreateMap<LinUserInputDto,LinUser>();
        }
    }
}
