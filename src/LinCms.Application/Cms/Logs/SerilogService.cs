using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Data;
using LinCms.Entities;
using LinCms.Extensions;

namespace LinCms.Cms.Logs
{
    public class SerilogService : ISerilogService
    {
        private IBaseRepository<SerilogDO> _serilogBaseRepository;

        public SerilogService(IBaseRepository<SerilogDO> serilogBaseRepository)
        {
            _serilogBaseRepository = serilogBaseRepository;
        }

        public async Task<PagedResultDto<SerilogDO>> GetListAsync(SerilogSearchDto searchDto)
        {
            List<SerilogDO> serilogLists = await _serilogBaseRepository.Select
                .WhereIf(searchDto.Start.HasValue, r => r.Timestamp >= searchDto.Start.Value)
                .WhereIf(searchDto.End.HasValue, r => r.Timestamp <= searchDto.End.Value)
                .WhereIf(!string.IsNullOrEmpty(searchDto.Keyword), r => r.Message.Contains(searchDto.Keyword))
                .WhereIf(searchDto.LogLevel!=null, r => r.Level==searchDto.LogLevel)
                .OrderByDescending(r => r.Id)
                .ToPagerListAsync(searchDto, out long count);
            return new PagedResultDto<SerilogDO>(serilogLists, count);
        }
    }
}
