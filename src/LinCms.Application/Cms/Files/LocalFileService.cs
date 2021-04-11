using LinCms.Common;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LinCms.Cms.Files
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IAuditBaseRepository<LinFile, long> _fileRepository;
        private readonly FileStorageOption _fileStorageOption;
        private readonly ICurrentUser _currentUser;
        public LocalFileService(IWebHostEnvironment hostingEnv, IAuditBaseRepository<LinFile, long> fileRepository, IOptions<FileStorageOption> fileStorageOption, ICurrentUser currentUser)
        {
            _hostingEnv = hostingEnv;
            _fileRepository = fileRepository;
            _fileStorageOption = fileStorageOption.Value;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 本地文件上传，生成目录格式 {PrefixPath}/{yyyyMM}/{guid}.文件后缀
        /// assets/202005/fba73a0c-f2f7-499a-8ed8-5b10554d43b0.jpg
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<Tuple<string, long>> LocalUploadAsync(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new LinCmsException("文件为空");
            }

            string saveFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            //得到 assets/202005
            string path = Path.Combine(_fileStorageOption.LocalFile.PrefixPath, DateTime.Now.ToString("yyyyMM"));
            //得到wwwroot/assets/202005
            string createFolder = Path.Combine(_hostingEnv.WebRootPath, path);
            //创建这种不存在的目录
            if (!Directory.Exists(createFolder))
            {
                Directory.CreateDirectory(createFolder);
            }

            long len = 0;
            await using (FileStream fs = File.Create(Path.Combine(createFolder, saveFileName)))
            {
                await file.CopyToAsync(fs);
                len = fs.Length;
                await fs.FlushAsync();
            }

            //windows下Path.Combine,得到的\\，不符号路径的要求。替换一下
            //得到 路径与文件大小    assets/202005/fba73a0c-f2f7-499a-8ed8-5b10554d43b0.jpg
            return Tuple.Create(Path.Combine(path, saveFileName).Replace("\\", "/"), len);
        }

        /// <summary>
        /// 本地文件上传，秒传（根据lin_file表中的md5,与当前文件的路径是否在本地），如果不在，重新上传，覆盖文件表记录
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<FileDto> UploadAsync(IFormFile file, int key = 0)
        {
            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());
            LinFile linFile = await _fileRepository.Where(r => r.Md5 == md5 && r.Type == 1).OrderByDescending(r => r.CreateTime).FirstAsync();

            if (linFile != null && File.Exists(Path.Combine(_hostingEnv.WebRootPath, linFile.Path)))
            {
                return new FileDto
                {
                    Id = linFile.Id,
                    Key = "file_" + key,
                    Path = linFile.Path,
                    Url = _fileStorageOption.LocalFile.Host + linFile.Path
                };
            }

            long id;

            var (path, len) = await this.LocalUploadAsync(file);

            if (linFile == null)
            {
                LinFile saveLinFile = new LinFile()
                {
                    Extension = Path.GetExtension(file.FileName),
                    Md5 = md5,
                    Name = file.FileName,
                    Path = path,
                    Type = 1,
                    Size = len
                };
                id = (await _fileRepository.InsertAsync(saveLinFile)).Id;
            }
            else
            {
                linFile.Path = path;
                await _fileRepository.UpdateAsync(linFile);
                id = linFile.Id;
            }

            return new FileDto
            {
                Id = id,
                Key = "file_" + key,
                Path = path,
                Url = _fileStorageOption.LocalFile.Host + path
            };
        }
    }
}
