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
        /// <returns></returns>
        [HttpPost("upload")]
        public FileDto Upload(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new LinCmsException("文件为空");
            }

            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South, //华南 
                UseHttps = _configuration[LinConsts.Qiniu.UseHttps].ToBoolean()
            });

            string fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName.Trim().ToString();

            string qiniuName = _configuration["Qiniu:PrefixPath"] + "/" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Path.GetExtension(fileName);
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, qiniuName, GetAccessToken(), null);

            if (result.Code != (int)HttpCode.OK) throw new LinCmsException("上传失败");

            return new FileDto
            {
                Path = qiniuName,
                Url = _configuration["Qiniu:Host"] + qiniuName
            };

        }

    }


}