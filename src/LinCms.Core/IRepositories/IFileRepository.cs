using LinCms.Entities;

namespace LinCms.IRepositories
{
    public interface IFileRepository : IAuditBaseRepository<LinFile>
    {
        string GetFileUrl(string path);

    }
}


