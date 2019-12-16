using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Cms.Files;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Services.Cms.Interfaces
{
    public interface IFileService
    {

        /// <summary>
        /// 单文件上传，键为file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        FileDto Upload(IFormFile file , int key = 0);
    }
}
