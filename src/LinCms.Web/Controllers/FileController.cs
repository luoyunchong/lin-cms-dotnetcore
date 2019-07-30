using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LinCms.Web.Models.Files;
using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
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
        public List<FileDto> UploadFiles(IFormFile file)
        {
            string domainUrl = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            Stream stream = file.OpenReadStream();

            string md5 = Utils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5).First();

            if (linFile != null)
            {
                return new List<FileDto>
                {
                    new FileDto()
                    {
                        Id=linFile.Id,
                        Key="file",
                        Path=linFile.Path,
                        Url=domainUrl+"/"+_configuration["File:StoreDir"]+"/"+linFile.Path
                    }
                };
            }

            string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().ToString();

            DateTime now = DateTime.Now;

            string newSaveName = Guid.NewGuid() + Path.GetExtension(filename);

            string savePath = Path.Combine(_hostingEnv.WebRootPath, _configuration["File:StoreDir"], now.ToString("yyy/MM/dd"));

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            int len = 0;

            using (FileStream fs = System.IO.File.Create(Path.Combine(savePath, newSaveName)))
            {
                file.CopyTo(fs);
                len = (int)fs.Length;

                fs.Flush();
            }
            LinFile saveLinFile = new LinFile()
            {
                Extension = Path.GetExtension(filename),
                Md5 = md5,
                Name = filename,
                Path = Path.Combine(now.ToString("yyy/MM/dd"), newSaveName).Replace("\\", "/"),
                Type = 1,
                CreateTime = DateTime.Now,
                Size = len
            };

            long id = _freeSql.Insert<LinFile>(saveLinFile).ExecuteIdentity();

            return new List<FileDto>
            {
                new FileDto()
                {
                    Id=(int)id,
                    Key="file",
                    Path=saveLinFile.Path,
                    Url=domainUrl+"/"+_configuration["File:StoreDir"]+"/"+saveLinFile.Path
                }
            };

        }
    }
}