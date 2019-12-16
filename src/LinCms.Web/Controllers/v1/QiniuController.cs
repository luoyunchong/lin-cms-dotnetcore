using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LinCms.Web.Models.Cms.Files;
using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    /// 七牛云上传服务
    /// </summary>
    [Route("v1/qiniu")]
    [ApiController]
    public class QiniuController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFreeSql _freeSql;

        public QiniuController(IFreeSql freeSql, IConfiguration configuration)
        {
            _configuration = configuration;
            _freeSql = freeSql;
        }

        /// <summary>
        /// 根据后台配置项，得到请求七牛云的token值，前台也可根据此token值上传至七牛云服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("access_token")]
        public string GetAccessToken()
        {
            Mac mac = new Mac(_configuration["Qiniu:AK"], _configuration["Qiniu:SK"]);
            PutPolicy putPolicy = new PutPolicy { Scope = _configuration["Qiniu:Bucket"] };
            return Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        }

        /// <summary>
        /// 上传文件至七牛云
        /// </summary>
        /// <param name="file">单个文件</param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public List<FileDto> Upload(IFormFile file, int key = 0)
        {
            if (file.Length == 0)
            {
                throw new LinCmsException("文件为空");
            }

            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5).First();

            if (linFile != null && linFile.Type == 1)
            {
                if (System.IO.File.Exists(linFile.Path))
                {
                    return new List<FileDto>
                    {
                        new FileDto
                        {
                            Id = linFile.Id,
                            Key = "file_" + key,
                            Path = linFile.Path,
                            Url = linFile.Path
                        }
                    };
                }
            }


            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South, //华南 
                UseHttps = true
            });

            Config config = new Config {Zone = Zone.ZONE_CN_East};

            string fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName.Trim().ToString();

            string qiniuName = _configuration["Qiniu:PrefixPath"] + "/" +
                               DateTime.Now.ToString("yyyyMMddHHmmssffffff") + fileName;
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, qiniuName, GetAccessToken(), null);

            if (result.Code != (int) HttpCode.OK) throw new LinCmsException("上传失败");
            long id;
            if (linFile==null)
            {
                Mac mac = new Mac(_configuration["Qiniu:AK"], _configuration["Qiniu:SK"]);
                BucketManager bucketManager = new BucketManager(mac, config);
                StatResult statRet = bucketManager.Stat(_configuration["Qiniu:Bucket"], qiniuName);
                long size = 0;
                if (statRet.Code == (int)HttpCode.OK)
                {
                    size = statRet.Result.Fsize;
                }
                else
                {
                    Console.WriteLine("stat error: " + statRet);
                }

                LinFile saveLinFile = new LinFile()
                {
                    Extension = Path.GetExtension(fileName),
                    Md5 = md5,
                    Name = fileName,
                    Path = qiniuName,
                    Type = 2,
                    CreateTime = DateTime.Now,
                    Size = size
                };

                id = _freeSql.Insert(saveLinFile).ExecuteIdentity();
            }
            else
            {
                id = linFile.Id;
            }

            return new List<FileDto>
            {
                new FileDto
                {
                    Id = (int) id,
                    Key = "file_" + key,
                    Path = qiniuName,
                    Url = _configuration["Qiniu:Host"] + qiniuName
                }
            };

        }

        /// <summary>
        /// 上传多文件至七牛云
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<FileDto> UploadFiles()
        {
            IFormFileCollection files = Request.Form.Files;
            List<FileDto> fileDtos = new List<FileDto>();
            files.ForEach((file, index) => { fileDtos.AddRange(this.Upload(file, index)); });
            return fileDtos;
        }
    }


}