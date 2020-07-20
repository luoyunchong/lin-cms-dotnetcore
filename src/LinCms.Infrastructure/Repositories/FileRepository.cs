using FreeSql;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.Extensions.Options;

namespace LinCms.Repositories
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