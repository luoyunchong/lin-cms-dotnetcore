using LinCms.Application.Contracts.Cms.Files.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Admins.Dtos;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Cms.Files
{
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
}
