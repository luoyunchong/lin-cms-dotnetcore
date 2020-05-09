using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Cms.Files;
using LinCms.Application.Contracts.Cms.Files;
using LinCms.Application.Contracts.Cms.Files.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/file")]
    [ApiController]
    //[Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<FileDto> UploadFiles()
        {
            IFormFileCollection files = Request.Form.Files;
            List<FileDto> fileDtos = new List<FileDto>();
            files.ForEach(async (file, index) =>
            {
                FileDto fileDto = await _fileService.UploadAsync(file, index);
                fileDtos.Add(fileDto);
            });
            return fileDtos;
        }

        /// <summary>
        /// 单文件上传，键为file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<FileDto> UploadAsync(IFormFile file, int key = 0)
        {
            return await _fileService.UploadAsync(file, key);
        }
    }
}