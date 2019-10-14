using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Qiniu.Web.Controllers.v1  
{
    /// <summary>
    /// 七牛云上传服务
    /// </summary>
    [Route("v1/qiniu")]
    [ApiController]
    public class QiniuController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public QiniuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 根据后台配置项，得到请求七牛云的token值，前台也可根据此token值上传至七牛云服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("access_token")]
        public QiniuResultDto GetAccessToken()
        {
            Mac mac = new Mac(_configuration["Qiniu:AK"], _configuration["Qiniu:SK"]);
            PutPolicy putPolicy = new PutPolicy { Scope = _configuration["Qiniu:Bucket"] };
            return new QiniuResultDto(0, "成功获取access_token", Auth.CreateUploadToken(mac, putPolicy.ToJsonString()));
        }

        /// <summary>
        /// 上传文件至七牛云,code为0，代表上传成功,其他代表不成功
        /// </summary>
        /// <param name="file">单个文件</param>
        /// <returns>new QiniuResultDto(0,"上传成功","七牛云文件地址，包括http://....mm.png");</returns>
        [HttpPost("upload")]
        public QiniuResultDto Upload(IFormFile file)
        {
            if (file.Length == 0)
            {
                return new QiniuResultDto(1, "文件为空");
            }

            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South,//华南 
                UseHttps = true
            });

            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName.Trim();

            string qiniuName = _configuration["Qiniu:PrefixPath"] + "/" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + fileName;
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, qiniuName, GetAccessToken().Data.ToString(), null);

            if (result.Code == 200)
            {
                return new QiniuResultDto(0, "上传成功", _configuration["Qiniu:Host"] + qiniuName);
            }

            return new QiniuResultDto(1, "上传失败");
        }

        /// <summary>
        /// 上传多文件至七牛云
        /// </summary>
        /// <param name="files">多个文件</param>
        /// <returns></returns>
        [HttpPost("upload-multifile")]
        public QiniuResultDto UploadMultifile(IFormFileCollection files)
        {
            if (files.Count == 0)
            {
                return new QiniuResultDto(1, "无文件");
            }

            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South,//华南 
                UseHttps = true
            });

            List<string> list = new List<string>();

            foreach (IFormFile file in files) //获取多个文件列表集合
            {
                var fileName = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName.Trim();

                string qiniuName = _configuration["Qiniu:PrefixPath"] + "/" +
                                   DateTime.Now.ToString("yyyyMMddHHmmssffffff") + fileName;
                Stream stream = file.OpenReadStream();
                HttpResult result = upload.UploadStream(stream, qiniuName, GetAccessToken().Data.ToString(), null);

                if (result.Code == 200)
                {
                    list.Add(_configuration["Qiniu:Host"] + qiniuName);
                }
                else
                {
                    return new QiniuResultDto(1, result.RefText);
                }
            }

            if (list.Count > 0)
            {
                return new QiniuResultDto(0, "上传成功", list);
            }

            return new QiniuResultDto(1, "上传失败", list);
        }
    }


    public class QiniuResultDto
    {
        public QiniuResultDto(int code, string msg, object data)
        {
            Code = code;
            Data = data;
            Msg = msg;
        }

        public QiniuResultDto(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        public int Code { get; set; }
        public object Data { get; set; }
        public string Msg { get; set; }
    }
}