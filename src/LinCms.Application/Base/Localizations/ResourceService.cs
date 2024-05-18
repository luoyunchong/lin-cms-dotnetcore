using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Exceptions;
using LinCms.Extensions;

namespace LinCms.Base.Localizations;

public class ResourceService(IAuditBaseRepository<LocalResource, long> resourceRepository)
    : ApplicationService, IResourceService
{
    public async Task DeleteAsync(long id)
    {
        await resourceRepository.DeleteAsync(new LocalResource { Id = id });
    }

    public async Task<PagedResultDto<ResourceDto>> GetListAsync(ResourceSearchDto searchDto)
    {
        List<ResourceDto> entity = (await resourceRepository.Select
                .Where(r => r.CultureId == searchDto.CultureId)
                .WhereIf(searchDto.Key.IsNotNullOrEmpty(), r => r.Key.Contains(searchDto.Key))
                .ToPagerListAsync(searchDto, out long totalCount))
            .Select(r =>
            {
                ResourceDto resourceDto = Mapper.Map<ResourceDto>(r);
                return resourceDto;
            }).ToList();

        return new PagedResultDto<ResourceDto>(entity, totalCount);
    }

    public async Task<ResourceDto> GetAsync(long id)
    {
        LocalResource entity = await resourceRepository.Select.Where(a => a.Id == id).ToOneAsync();
        ResourceDto resourceDto = Mapper.Map<ResourceDto>(entity);
        return resourceDto;
    }

    public async Task CreateAsync(ResourceDto resourceDto)
    {
        bool exist = await resourceRepository.Select.AnyAsync(r => r.Key == resourceDto.Key && r.CultureId == resourceDto.CultureId);
        if (exist)
        {
            throw new LinCmsException($"Key[{resourceDto.Key}]已存在");
        }

        LocalResource entity = Mapper.Map<LocalResource>(resourceDto);
        await resourceRepository.InsertAsync(entity);
    }

    public async Task UpdateAsync(ResourceDto resourceDto)
    {
        LocalResource entity = await resourceRepository.Select.Where(r => r.Id == resourceDto.Id).ToOneAsync();
        if (entity == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        bool exist = await resourceRepository.Select.AnyAsync(r => r.Key == resourceDto.Key && r.Id != resourceDto.Id && r.CultureId == resourceDto.CultureId);
        if (exist)
        {
            throw new LinCmsException($"Key[{resourceDto.Key}]已存在");
        }

        Mapper.Map(resourceDto, entity);
        await resourceRepository.UpdateAsync(entity);
    }
}