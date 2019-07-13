using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.Users;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Users
{
    public class LinUserProfile : Profile
    {
        public LinUserProfile()
        {
            CreateMap<LinUserInputDto,LinUser>();
            CreateMap<LinUser, LinUserInformation>();
        }
    }
}
