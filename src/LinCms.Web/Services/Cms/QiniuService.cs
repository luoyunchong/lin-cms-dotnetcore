using System;
using System.IO;
using System.Security.Cryptography;
using LinCms.Web.Models.Cms.Files;
using LinCms.Web.Services.Cms.Interfaces;
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

namespace LinCms.Web.Services.Cms
{
    public class QiniuService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IFreeSql _freeSql;

        public QiniuService(IConfiguration configuration, IFreeSql freeSql)
        {
            _configuration = configuration;
            _freeSql = freeSql;
        }

        private string GetAccessToken()
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
        public FileDto Upload(IFormFile file, int key = 0)
        {
            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            Config config = new Config
            {
                Zone = Zone.ZONE_CN_South,
                UseHttps = true
            };
            FormUploader upload = new FormUploader(config);

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5 && r.Type == 2).First();

            if (linFile != null)
            {
                Mac mac = new Mac(_configuration["Qiniu:AK"], _configuration["Qiniu:SK"]);
                BucketManager bucketManager = new BucketManager(mac, config);
                StatResult statRet = bucketManager.Stat(_configuration["Qiniu:Bucket"], linFile.Path);
                if (statRet.Code == (int)HttpCode.OK)
                {
                    return new FileDto
                    {
                        Id = linFile.Id,
                        Key = "file_" + key,
                        Path = linFile.Path,
                        Url = _configuration["Qiniu:Host"] + linFile.Path
                    };
                }

                Console.WriteLine("stat error: " + statRet);
            }


            string fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName.Trim().ToString();

            string qiniuName = _configuration["Qiniu:PrefixPath"] + "/" +
                               DateTime.Now.ToString("yyyyMMddHHmmssffffff") + fileName;
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, qiniuName, GetAccessToken(), null);

            if (result.Code != (int)HttpCode.OK) throw new LinCmsException("上传失败");

            long size = 0;
            long id;
            if (linFile == null)
            {
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
                _freeSql.Update<LinFile>(linFile.Id).Set(a => a.Path, qiniuName).ExecuteAffrows();
                id = linFile.Id;
            }

            return new FileDto
            {
                Id = (int)id,
                Key = "file_" + key,
                Path = qiniuName,
                Url = _configuration["Qiniu:Host"] + qiniuName
            };

        }

    }
}
