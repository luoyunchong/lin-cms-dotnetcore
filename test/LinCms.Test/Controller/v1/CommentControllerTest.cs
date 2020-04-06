using AutoMapper;
using FreeSql;
using LinCms.Core.Entities.Blog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test.Controller.v1
{
    public class CommentControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly BaseRepository<Comment> _baseRepository;

        public CommentControllerTest() : base()
        {
            _hostingEnv = ServiceProvider.GetService<IWebHostEnvironment>();

            _mapper = ServiceProvider.GetService<IMapper>();
            _baseRepository = ServiceProvider.GetService<BaseRepository<Comment>>();
            _freeSql = ServiceProvider.GetService<IFreeSql>();
        }


    }
}
