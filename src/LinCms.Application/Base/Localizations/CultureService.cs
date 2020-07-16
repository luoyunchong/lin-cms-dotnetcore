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

        public async Task<CultureDto> CreateAsync(CultureDto cultureDto)
        {
            bool exist = await _cultureRepository.Select.AnyAsync(r => r.Name == cultureDto.Name);
            if (exist)
            {
                throw new LinCmsException($"Name[{cultureDto.Name}]已存在");
            }

            LocalCulture localCulture = _mapper.Map<LocalCulture>(cultureDto);
            await _cultureRepository.InsertAsync(localCulture);
            return _mapper.Map<CultureDto>(localCulture);
        }

        public async Task<CultureDto> UpdateAsync(CultureDto cultureDto)
        {
            LocalCulture localCulture = await _cultureRepository.Select.Where(r => r.Id == cultureDto.Id).ToOneAsync();
            if (localCulture == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = await _cultureRepository.Select.AnyAsync(r => r.Name == cultureDto.Name && r.Id != cultureDto.Id);
            if (exist)
            {
                throw new LinCmsException($"Name[{cultureDto.Name}]已存在");
            }

            _mapper.Map(cultureDto, localCulture);
            await _cultureRepository.UpdateAsync(localCulture);
            return _mapper.Map<CultureDto>(localCulture);
        }
    }
}
