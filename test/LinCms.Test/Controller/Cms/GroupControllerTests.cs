﻿using System;
using System.Collections.Generic;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Core.Entities;
using LinCms.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.Cms
{
    public class GroupControllerTests 
    {
        private readonly IMapper _mapper;

        public GroupControllerTests() 
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper((typeof(Startup).Assembly));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            _mapper = serviceProvider.GetService<IMapper>();

        }

        [Fact]
        public void IMapper()
        {
            CreateGroupDto inputDto = new CreateGroupDto()
            {
                Auths = new List<string>()
                {
                    "查询日志记录的用户"
                },
                Info = "11",
                Name = "11"
            };

            UpdateGroupDto update = new UpdateGroupDto()
            {
                Info = "11",
                Name = "11"
            };

            LinGroup d = _mapper.Map<LinGroup>(inputDto);
            LinGroup d2 = _mapper.Map<LinGroup>(update);

        }
    }
}
