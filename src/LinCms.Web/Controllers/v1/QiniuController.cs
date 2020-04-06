using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Cms.Files;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Application.Contracts.Cms.Files;
using LinCms.Application.Contracts.Cms.Files.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    /// 七牛云上传服务
    /// </summary>
    [Route("v1/qiniu")]
    [ApiController]
    public class QiniuController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ITagService _tagService;
        private readonly IAuditBaseRepository<Tag> _tagAuditBaseRepository;
        public QiniuController( IWebHostEnvironment hostingEnv, IFileService fileService, ITagService tagService, IAuditBaseRepository<Tag> tagAuditBaseRepository)
        {
            _hostingEnv = hostingEnv;
            _fileService = fileService;
            _tagService = tagService;
            _tagAuditBaseRepository = tagAuditBaseRepository;
        }

        /// <summary>
        /// 将掘金中的取所有标签存到七牛云上，基本信息存入数据库
        /// 从百度云下载后，放到wwwwroot中，swagger上执行下此方法，需要等很久，提取其中的tags，将Icon上传到七牛云上，tag信息存到数据库中。
        /// 链接: https://pan.baidu.com/s/1f7DSCC3uNOyiu3st9F1OAQ 提取码: paqf 
        /// </summary>
        /// <returns></returns>
        [HttpPost("tag")]
        public async Task<UnifyResponseDto> UploadTagByJson()
        {
            string tagPath = Path.Combine(_hostingEnv.WebRootPath, "json-tag.json");
            string text = System.IO.File.ReadAllText(tagPath);

            JObject json = JsonConvert.DeserializeObject<JObject>(text);

            foreach (var tag in json["d"]["tags"])
            {

                string tagName = tag["title"].ToString();
                bool valid = await _tagAuditBaseRepository.Where(r => r.TagName == tagName).AnyAsync();

                if (valid)
                {
                    Console.WriteLine($"{tagName}已存在，不需要生成");
                    continue;
                }

                FileDto fileDto = this.UploadToQiniu(tag["icon"].ToString());

                var tagEntity = new CreateUpdateTagDto()
                {
                    TagName = tagName,
                    Alias = tag["alias"].ToString(),
                    Status = true,
                    Thumbnail = fileDto.Path
                };
                await _tagService.CreateAsync(tagEntity);

            }

            return UnifyResponseDto.Success();
        }

        private FileDto UploadToQiniu(string remoteImagePath)
        {
            IFormFile file = GetUrlFormFile(remoteImagePath);
            return _fileService.Upload(file);
        }


        /// <summary>
        /// 获取远程服务器内容，并转换成流
        /// </summary>
        /// <param name="path">https://lc-gold-cdn.xitu.io/bac28828a49181c34110.png</param>
        /// <returns></returns>
        private IFormFile GetUrlFormFile(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            List<byte> btlst = new List<byte>();
            int b = responseStream.ReadByte();
            while (b > -1)
            {
                btlst.Add((byte)b);
                b = responseStream.ReadByte();
            }
            byte[] bts = btlst.ToArray();
            var ms = new MemoryStream();
            ms.Seek(0, SeekOrigin.Begin);
            ms.Write(bts);
            string fileName = path.Substring(path.LastIndexOf("/") + 1, path.Length - path.LastIndexOf("/") - 1);
            return new FormFile(ms, 0, bts.Length, "file", fileName);
        }
    }
}