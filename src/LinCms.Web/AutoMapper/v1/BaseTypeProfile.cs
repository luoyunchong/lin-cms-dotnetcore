using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.v1.BaseItems;
using LinCms.Web.Models.v1.BaseTypes;
using LinCms.Zero.Domain.Base;

namespace LinCms.Web.AutoMapper.v1
{
    public class BaseTypeProfile:Profile
    {
        public BaseTypeProfile()
        {
            CreateMap<BaseType, BaseTypeDto>();
            CreateMap<CreateUpdateBaseTypeDto, BaseType>();
        }
    }
}
