using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities;

namespace LinCms.IRepositories
{
    public interface IFileRepository : IAuditBaseRepository<LinFile, long>
    {
        string GetFileUrl(string path);

    }
}


