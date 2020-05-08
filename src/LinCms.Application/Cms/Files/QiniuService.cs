using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Files;
using LinCms.Application.Contracts.Cms.Files.Dtos;
using LinCms.Core.Common;
using LinCms.Core.Data.Options;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace LinCms.Application.Cms.Files
{
    public class QiniuService : IFileService
    {
        private readonly IAuditBaseRepository<LinFile> _fileRepository;
        private QiniuOptions _qiniuOptions;

        public QiniuService(IAuditBaseRepository<LinFile> fileRepository, IOptions<QiniuOptions> options)
        {
            _fileRepository = fileRepository;
            _qiniuOptions = options.Value;
        }

        private string GetAccessToken()
        {
            Mac mac = new Mac(_qiniuOptions.AK, _qiniuOptions.SK);
            PutPolicy putPolicy = new PutPolicy { Scope = _qiniuOptions.Bucket };
            return Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        }

        /// <summary>
        /// 七牛云上传 {PrefixPath}/{yyyyMMddHHmmssffffff}.文件后缀
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string QiniuUpload(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new LinCmsException("文件为空");
            }

            FormUploader upload = new FormUploader(new Config()
            {
                Zone = Zone.ZONE_CN_South,
                UseHttps = _qiniuOptions.UseHttps
            });

            string path = _qiniuOptions.PrefixPath + "/" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Path.GetExtension(file.FileName);
            Stream stream = file.OpenReadStream();
            HttpResult result = upload.UploadStream(stream, path, GetAccessToken(), null);
            if (result.Code != (int)HttpCode.OK) throw new LinCmsException("上传失败");
            return path;

        }

        /// <summary>
        /// 上传文件至七牛云，如果本地存在这条记录，直接返回文件的信息
        /// </summary>
        /// <param name="file">单个文件</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<FileDto> UploadAsync(IFormFile file, int key = 0)
        {
            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = await _fileRepository.Where(r => r.Md5 == md5 && r.Type == 2).FirstAsync();

            if (linFile != null)
            {
                return new FileDto
                {
                    Id = linFile.Id,
                    Key = "file_" + key,
                    Path = linFile.Path,
                    Url = _qiniuOptions.Host + linFile.Path
                };
            }

            string path = this.QiniuUpload(file);

            LinFile saveLinFile = new LinFile()
            {
                Extension = Path.GetExtension(file.FileName),
                Md5 = md5,
                Name = file.FileName,
                Path = path,
                Type = 2,
                Size = file.Length,
            };

            long id = (await _fileRepository.InsertAsync(saveLinFile)).Id;

            return new FileDto
            {
                Id = id,
                Key = "file_" + key,
                Path = path,
                Url = _qiniuOptions.Host + "/" + path
            };

        }

    }
}
