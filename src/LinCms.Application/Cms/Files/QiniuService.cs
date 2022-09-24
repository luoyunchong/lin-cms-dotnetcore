using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LinCms.Common;
using LinCms.Data.Options;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities;
using LinCms.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace LinCms.Cms.Files;

[DisableConventionalRegistration]
public class QiniuService : IFileService
{
    private readonly IAuditBaseRepository<LinFile> _fileRepository;
    private readonly FileStorageOption _fileStorageOption;

    public QiniuService(IAuditBaseRepository<LinFile> fileRepository, IOptions<FileStorageOption> fileStorageOption)
    {
        _fileRepository = fileRepository;
        _fileStorageOption = fileStorageOption.Value;
    }

    private string GetAccessToken()
    {
        Mac mac = new(_fileStorageOption.Qiniu.AK, _fileStorageOption.Qiniu.SK);
        PutPolicy putPolicy = new() { Scope = _fileStorageOption.Qiniu.Bucket };
        return Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
    }

    /// <summary>
    /// 七牛云上传 {PrefixPath}/{yyyyMM}/{guid}.文件后缀
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private string QiniuUpload(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new LinCmsException("文件为空");
        }

        FormUploader upload = new(new Config()
        {
            Zone = Zone.ZONE_CN_South,
            UseHttps = _fileStorageOption.Qiniu.UseHttps
        });

        string path = _fileStorageOption.Qiniu.PrefixPath + "/" + DateTime.Now.ToString("yyyyMM") + "/" + Guid.NewGuid() + Path.GetExtension(file.FileName);
        using Stream stream = file.OpenReadStream();
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
                Url = _fileStorageOption.Qiniu.Host + linFile.Path
            };
        }

        string path = QiniuUpload(file);

        LinFile saveLinFile = new()
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
            Url = _fileStorageOption.Qiniu.Host + path
        };

    }

}