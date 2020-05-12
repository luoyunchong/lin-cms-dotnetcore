using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Data.Options;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.Extensions.Options;

namespace LinCms.Infrastructure.Repositories
{
    public class FileRepository : AuditBaseRepository<LinFile>, IFileRepository
    {
        private readonly FileStorageOption _fileStorageOption;
        public FileRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser, IOptions<FileStorageOption> fileStorageOption): base(unitOfWorkManager, currentUser)
        {
            _fileStorageOption = fileStorageOption.Value;
        }

        public string GetFileUrl(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            if (path.StartsWith("http")||path.StartsWith("https"))
            {
                return path;
            }

            LinFile linFile= base.Where(r => r.Path == path).First();
            if (linFile == null) return path;
            switch (linFile.Type)
            {
                case 1: 
                    return _fileStorageOption.LocalFile.Host + path;
                case 2: 
                    return  _fileStorageOption.Qiniu.Host + path;
                default:
                    return path;
            }
        }
    }
}