using System;
using System.IO;
using System.Security.Cryptography;
using LinCms.Application.Contracts.Cms.Files;
using LinCms.Application.Contracts.Cms.Files.Dtos;
using LinCms.Core.Common;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace LinCms.Application.Cms.Files
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
            Mac mac = new Mac(_configuration[LinConsts.Qiniu.AK], _configuration[LinConsts.Qiniu.SK]);
            PutPolicy putPolicy = new PutPolicy { Scope = _configuration[LinConsts.Qiniu.Bucket] };
            return Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        }

        /// <summary>
        /// 上传文件至七牛云
        /// </summary>
        /// <param name="file">单个文件</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileDto Upload(IFormFile file, int key = 0)
        {
            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5 && r.Type == 2).First();

            if (linFile != null)
            {
                return new FileDto
                {
                    Id = linFile.Id,
                    Key = "file_" + key,
                    Path = linFile.Path,
                    Url = _configuration[LinConsts.Qiniu.Host] + linFile.Path
                };
            }

            string extension = Path.GetExtension(file.FileName);

            string path = this.UploadToQiniu(file);

            long size = 0;
            LinFile saveLinFile = new LinFile()
            {
                Extension = extension,
                Md5 = md5,
                Name = file.FileName,
                Path = path,
                Type = 2,
                CreateTime = DateTime.Now,
                Size = size
            };

            long id = _freeSql.Insert(saveLinFile).ExecuteIdentity();
            return new FileDto
            {
                Id = (int)id,
                Key = "file_" + key,
                Path = path,
                Url = _configuration[LinConsts.Qiniu.Host] + path
            };

        }


        public string UploadToQiniu(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new LinCmsException("文件为空");
            }

            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South,
                UseHttps = _configuration[LinConsts.Qiniu.UseHttps].ToBoolean()
            });

            string path = _configuration["Qiniu:PrefixPath"] + "/" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Path.GetExtension(file.FileName);
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, path, GetAccessToken(), null);
            if (result.Code != (int) HttpCode.OK) throw new LinCmsException("上传失败");
            return path;

        }
    }
}
