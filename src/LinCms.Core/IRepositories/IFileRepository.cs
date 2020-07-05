using LinCms.Core.Entities;

namespace LinCms.Core.IRepositories
{
    public interface IFileRepository : IAuditBaseRepository<LinFile>
    {
        string GetFileUrl(string path);

    }
}


