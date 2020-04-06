using System.Collections.Generic;
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
        /// 上传多文件至本地或七牛云，swagger无法正常生成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<FileDto> UploadFiles()
        {
            IFormFileCollection files = Request.Form.Files;
            List<FileDto> fileDtos = new List<FileDto>();
            files.ForEach((file, index) => { fileDtos.Add(_fileService.Upload(file, index)); });
            return fileDtos;
        }

        /// <summary>
        /// 单文件上传，键为file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <returns></returns>
       [HttpPost("upload")]
        public FileDto Upload(IFormFile file, int key = 0)
        {
            return _fileService.Upload(file, key);
        }
    }
}