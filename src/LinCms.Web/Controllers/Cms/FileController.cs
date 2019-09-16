using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LinCms.Web.Models.Cms.Files;
using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/file")]
    [ApiController]
    //[Authorize]
    public class FileController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IFreeSql _freeSql;
        private readonly IConfiguration _configuration;

        public FileController(IHostingEnvironment hostingEnv, IFreeSql freeSql, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _freeSql = freeSql;
            _configuration = configuration;
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<FileDto> UploadFiles()
        {
            var files = Request.Form.Files;
            List<FileDto> fileDtos = new List<FileDto>();
            files.ForEach((file,index) => { fileDtos.AddRange(this.Upload(file, index)); });
            return fileDtos;
        }

        /// <summary>
        /// 单文件上传，键为file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("/upload")]
        public List<FileDto> Upload(IFormFile file,int key=0)
        {
            string domainUrl = _configuration["SITE_DOMAIN"];
            string fileDir = _configuration["FILE:STORE_DIR"];

            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5).First();

            if (linFile != null)
            {
                return new List<FileDto>
                {
                    new FileDto
                    {
                        Id=linFile.Id,
                        Key="file_"+key,
                        Path=linFile.Path,
                        Url=domainUrl + "/" +_configuration["FILE:STORE_DIR"]+"/"+linFile.Path
                    }
                };
            }

            string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().ToString();

            DateTime now = DateTime.Now;

            string newSaveName = Guid.NewGuid() + Path.GetExtension(filename);

            string savePath = Path.Combine(_hostingEnv.WebRootPath, fileDir, now.ToString("yyy/MM/dd"));

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            int len;

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

            long id = _freeSql.Insert(saveLinFile).ExecuteIdentity();

            return new List<FileDto>
            {
                new FileDto
                {
                    Id=(int)id,
                    Key="file_"+key,
                    Path=saveLinFile.Path,
                    Url=domainUrl + "/" +fileDir+"/"+saveLinFile.Path
                }
            };

        }
    }
}