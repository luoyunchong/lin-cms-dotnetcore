using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace LinCms.Web.Controllers
{
    [Route("cms/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IFreeSql _freeSql;
        private readonly IConfiguration _configuration;

        public FileController(IHostingEnvironment hostingEnv, IFreeSql freeSql, IConfiguration configuration)
        {
            this._hostingEnv = hostingEnv;
            _freeSql = freeSql;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult UploadFiles(IFormFile file)
        {
            string filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim().ToString();

            DateTime now = DateTime.Now;

            string newSaveName = Guid.NewGuid() + Path.GetExtension(filename);

            string saveName = _hostingEnv.WebRootPath + Path.Combine(_configuration["StoreDir"], now.Year.ToString(), now.Month.ToString(), now.Day.ToString(), newSaveName);

            using (FileStream fs = System.IO.File.Create(saveName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            return Ok(newSaveName);

        }
    }
}