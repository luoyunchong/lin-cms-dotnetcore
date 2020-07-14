using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Application.Contracts.Base.Cultures
{
    public interface ICultureService
    {
        Task<List<CultureDto>> GetListAsync();

        Task DeleteAsync(long id);

        Task<CultureDto> GetAsync(long id);

        Task CreateAsync(CultureDto createChannel);

        Task UpdateAsync(CultureDto updateChannel);
    }
}
