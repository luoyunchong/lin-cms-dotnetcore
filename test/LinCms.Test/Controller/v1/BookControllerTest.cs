using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class BookControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly BaseRepository<Book> _baseRepository;

        public BookControllerTest() : base()
        {
            _hostingEnv = GetService<IWebHostEnvironment>();

            _mapper = GetService<IMapper>();
            _baseRepository = GetService<BaseRepository<Book>>();
            _freeSql = GetService<IFreeSql>();
        }

        
        [Fact]
        public async Task CreateAsync()
        {
            // Act
            var response = await Client.GetAsync($"/v1/book");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutAsync()
        {
        }

        [Fact]
        public async Task Get( )
        {
            // Act
            var response = await Client.GetAsync($"/v1/book");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAsync()
        {
        }

        [Fact]
        public void DeleteBook()
        {
        }


    }
}
