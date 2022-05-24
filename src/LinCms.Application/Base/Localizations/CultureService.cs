using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Base.Localizations
{
    public class CultureService : ApplicationService, ICultureService
    {
        private readonly IAuditBaseRepository<LocalCulture, long> _cultureRepository;
        public CultureService(IAuditBaseRepository<LocalCulture, long> cultureRepository)
        {
            _cultureRepository = cultureRepository;
        }

        public async Task DeleteAsync(long id)
        {
            await _cultureRepository.DeleteAsync(new LocalCulture { Id = id });
        }

        public async Task<List<CultureDto>> GetListAsync()
        {
            List<LocalCulture> entities = await _cultureRepository.Select.ToListAsync();
            return Mapper.Map<List<CultureDto>>(entities);
        }

        public async Task<CultureDto> GetAsync(long id)
        {
            LocalCulture entity = await _cultureRepository.Select
                .Where(a => a.Id == id).ToOneAsync();

            CultureDto resourceDto = Mapper.Map<CultureDto>(entity);
            return resourceDto;
        }

        public async Task<CultureDto> CreateAsync(CultureDto cultureDto)
        {
            bool exist = await _cultureRepository.Select.AnyAsync(r => r.Name == cultureDto.Name);
            if (exist)
            {
                throw new LinCmsException($"Name[{cultureDto.Name}]已存在");
            }

            LocalCulture localCulture = Mapper.Map<LocalCulture>(cultureDto);
            await _cultureRepository.InsertAsync(localCulture);
            return Mapper.Map<CultureDto>(localCulture);
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

            Mapper.Map(cultureDto, localCulture);
            await _cultureRepository.UpdateAsync(localCulture);
            return Mapper.Map<CultureDto>(localCulture);
        }
    }
}
