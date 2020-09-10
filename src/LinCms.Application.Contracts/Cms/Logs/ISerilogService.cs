using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Logs
{
    public interface ISerilogService
    {
        Task<LogDashboard> GetLogDashboard();
        Task<PagedResultDto<SerilogDO>> GetListAsync(SerilogSearchDto searchDto);
    }
}
