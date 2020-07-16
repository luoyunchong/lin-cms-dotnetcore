using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Localizations;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Base.Localizations
{
    public class ResourceService : IResourceService
    {
        private readonly IAuditBaseRepository<LocalResource, long> _resourceRepository;
        private readonly IMapper _mapper;

        public ResourceService(IMapper mapper, IAuditBaseRepository<LocalResource, long> resourceRepository)
        {
            _mapper = mapper;
            _resourceRepository = resourceRepository;
        }

        public async Task DeleteAsync(long id)
        {
            await _resourceRepository.DeleteAsync(new LocalResource { Id = id });
        }

        public async Task<PagedResultDto<ResourceDto>> GetListAsync(ResourceSearchDto searchDto)
        {
            List<ResourceDto> entity = (await _resourceRepository.Select
                    .Where(r => r.CultureId == searchDto.CultureId)
                    .WhereIf(searchDto.Key.IsNotNullOrEmpty(), r => r.Key.Contains(searchDto.Key))
                    .ToPagerListAsync(searchDto, out long totalCount))
                    .Select(r =>
                    {
                        ResourceDto resourceDto = _mapper.Map<ResourceDto>(r);
                        return resourceDto;
                    }).ToList();

            return new PagedResultDto<ResourceDto>(entity, totalCount);
        }

        public async Task<ResourceDto> GetAsync(long id)
        {
            LocalResource entity = await _resourceRepository.Select.Where(a => a.Id == id).ToOneAsync();
            ResourceDto resourceDto = _mapper.Map<ResourceDto>(entity);
            return resourceDto;
        }

        public async Task CreateAsync(ResourceDto resourceDto)
        {
            bool exist = await _resourceRepository.Select.AnyAsync(r => r.Key == resourceDto.Key && r.CultureId == resourceDto.CultureId);
            if (exist)
            {
                throw new LinCmsException($"Key[{resourceDto.Key}]已存在");
            }

            LocalResource entity = _mapper.Map<LocalResource>(resourceDto);
            await _resourceRepository.InsertAsync(entity);
        }

        public async Task UpdateAsync(ResourceDto resourceDto)
        {
            LocalResource entity = await _resourceRepository.Select.Where(r => r.Id == resourceDto.Id).ToOneAsync();
            if (entity == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = await _resourceRepository.Select.AnyAsync(r => r.Key == resourceDto.Key && r.Id != resourceDto.Id && r.CultureId == resourceDto.CultureId);
            if (exist)
            {
                throw new LinCmsException($"Key[{resourceDto.Key}]已存在");
            }

            _mapper.Map(resourceDto, entity);
            await _resourceRepository.UpdateAsync(entity);
        }
    }
}
