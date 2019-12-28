using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Comments;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();

            _mapper = serviceProvider.GetService<IMapper>();
            _baseRepository = serviceProvider.GetService<BaseRepository<Comment>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
        }


    }
}
