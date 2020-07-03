using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Logs.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Logs
{
    public interface ISerilogService
    {
        Task<PagedResultDto<SerilogDO>> GetListAsync(SerilogSearchDto searchDto);
    }
}
