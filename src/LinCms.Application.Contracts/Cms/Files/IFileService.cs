using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LinCms.Cms.Files;

/// <summary>
/// 文件服务
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 单文件上传，键为file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<FileDto> UploadAsync(IFormFile file, int key = 0);

}