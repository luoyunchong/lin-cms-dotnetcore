using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using System;
using LinCms.Core.Entities.Blog;
using LinCms.Infrastructure.Repositories;
using LinCms.Web.Controllers.Blog;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class TagControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly TagController _tagController;

        public TagControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();
            _tagController = serviceProvider.GetService<TagController>(); ;

            _mapper = serviceProvider.GetService<IMapper>();
            _tagRepository = serviceProvider.GetService<AuditBaseRepository<Tag>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
        }

        [Fact]
        public void CorrectedTagCount()
        {
            Guid tagId = new Guid("5dc931fd-5e44-c190-008e-3fc4728735d6");
            _tagController.CorrectedTagCount(tagId);
        }
    }
}
