using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.IRepositories;
using Microsoft.Extensions.Options;

namespace LinCms.Repositories;

public class FileRepository(UnitOfWorkManager unitOfWorkManager,
        ICurrentUser currentUser,
        IOptionsMonitor<FileStorageOption> fileStorageOption)
    : AuditDefaultRepository<LinFile, long, long>(unitOfWorkManager, currentUser), IFileRepository
{
    private readonly FileStorageOption _fileStorageOption = fileStorageOption.CurrentValue;

    public string GetFileUrl(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        if (path.StartsWith("http") || path.StartsWith("https"))
        {
            return path;
        }
        return _fileStorageOption.LocalFile.Host + path;

        //string redisKey = "filerepository:getfileurl:" +path;

        //return  RedisHelper.CacheShell(
        //    redisKey, 5*60, () =>
        //    {
        //        LinFile linFile = Where(r => r.Path == path).First();
        //        if (linFile == null) return path;
        //        return linFile.Type switch
        //        {
        //            1 => _fileStorageOption.LocalFile.Host + path,
        //            2 => _fileStorageOption.Qiniu.Host + path,
        //            _ => path,
        //        };
        //    }
        // );
    }
}