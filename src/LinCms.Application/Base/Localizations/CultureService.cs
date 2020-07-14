using AutoMapper;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Cultures;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Application.Base.Localizations
{
    public class CultureService : ICultureService
    {
        private readonly IMapper _mapper;
        private readonly IAuditBaseRepository<LocalCulture, long> _cultureRepository;
        public CultureService(IMapper mapper, IAuditBaseRepository<LocalCulture, long> cultureRepository)
        {
            _mapper = mapper;
            _cultureRepository = cultureRepository;
        }

        public async Task DeleteAsync(long id)
        {
            await _cultureRepository.DeleteAsync(new LocalCulture { Id = id });
        }

        public async Task<List<CultureDto>> GetListAsync()
        {
            List<LocalCulture> entities = await _cultureRepository.Select.ToListAsync();
            return _mapper.Map<List<CultureDto>>(entities);
        }

        public async Task<CultureDto> GetAsync(long id)
        {
            LocalCulture entity = await _cultureRepository.Select
                .Where(a => a.Id == id).ToOneAsync();

            CultureDto resourceDto = _mapper.Map<CultureDto>(entity);
            return resourceDto;
        }

        public async Task CreateAsync(CultureDto createCulture)
        {
            bool exist = await _cultureRepository.Select.AnyAsync(r => r.Name == createCulture.Name);
            if (exist)
            {
                throw new LinCmsException($"Name[{createCulture.Name}]已存在");
            }

            LocalCulture entity = _mapper.Map<LocalCulture>(createCulture);
            await _cultureRepository.InsertAsync(entity);
        }

        public async Task UpdateAsync(CultureDto updateCulture)
        {
            LocalCulture entity = await _cultureRepository.Select.Where(r => r.Id == updateCulture.Id).ToOneAsync();
            if (entity == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = await _cultureRepository.Select.AnyAsync(r => r.Name == updateCulture.Name && r.Id != updateCulture.Id);
            if (exist)
            {
                throw new LinCmsException($"Name[{updateCulture.Name}]已存在");
            }

            _mapper.Map(updateCulture, entity);
            await _cultureRepository.UpdateAsync(entity);
        }
    }
}
