using System.Collections.Generic;
using LinCms.Web.Models.Cms.Files;
using LinCms.Web.Services.Cms.Interfaces;
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
        /// 上传多文件至七牛云
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<FileDto> UploadFiles(IFormFileCollection files)
        {
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