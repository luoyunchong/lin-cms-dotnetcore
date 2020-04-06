using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using LinCms.Core.Entities.Blog;
using LinCms.Web.Controllers.Blog;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using LinCms.Core.IRepositories;

namespace LinCms.Test.Controller.v1
{
    public class TagControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly TagController _tagController;

        public TagControllerTest() : base()
        {
            _hostingEnv = ServiceProvider.GetService<IWebHostEnvironment>();
            _tagController = ServiceProvider.GetService<TagController>(); ;
            _mapper = ServiceProvider.GetService<IMapper>();
            _tagRepository = ServiceProvider.GetService<IAuditBaseRepository<Tag>>();
            _freeSql = ServiceProvider.GetService<IFreeSql>();
        }

        [Fact]
        public async Task CorrectedTagCountAsync()
        {
            Guid tagId = new Guid("5dc931fd-5e44-c190-008e-3fc4728735d6");
            await _tagController.CorrectedTagCountAsync(tagId);
        }
    }
}
