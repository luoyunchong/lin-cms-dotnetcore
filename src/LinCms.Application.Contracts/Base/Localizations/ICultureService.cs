using IGeekFan.Localization.FreeSql.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Application.Contracts.Base.Cultures
{
    interface ICultureService
    {
        Task<List<LocalCulture>> GetListAsync();

        Task DeleteAsync(long id);

        Task<LocalCulture> GetAsync(long id);

        Task CreateAsync(LocalCulture createChannel);

        Task UpdateAsync(LocalCulture updateChannel);
    }
}
