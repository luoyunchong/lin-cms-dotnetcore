﻿using LinCms.Application.Contracts.Cms.Files;
using Microsoft.AspNetCore.Http;

namespace LinCms.Application.Cms.Files
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
