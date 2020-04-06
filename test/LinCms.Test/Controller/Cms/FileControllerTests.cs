using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.Cms
{
    public class FileControllerTests : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;

        public FileControllerTests() : base()
        {
            _hostingEnv = ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        }

        [Fact]
        public void FileStreamCreateTest()
        {
            string fileName = Guid.NewGuid() + Path.GetExtension("1212121.png");


            DateTime now = DateTime.Now;
            string savePath = _hostingEnv.WebRootPath + Path.Combine(now.Year.ToString(), now.Month.ToString(), now.Day.ToString());

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string path = Path.Combine(savePath, fileName);

            using (FileStream fs = File.Create(path))
            {

            }
        }

        [Fact]
        public void DateMonthTest()
        {
            DateTime now =new DateTime(2019,2,3);

            string d= now.ToString("yyy/MM/dd");

            Assert.Equal("2019/02/03", d);
        }
    }
}
