using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Logs
{
    public interface ISerilogService
    {
        Task<PagedResultDto<SerilogDO>> GetListAsync(SerilogSearchDto searchDto);
    }
}
