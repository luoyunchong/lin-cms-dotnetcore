using System;
using AutoMapper;
using LinCms.Application;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
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
            services.AddAutoMapper((typeof(ApplicationService).Assembly));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            _mapper = serviceProvider.GetService<IMapper>();

        }

        [Fact]
        public void IMapper()
        {
            CreateGroupDto inputDto = new CreateGroupDto()
            {
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
