using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Filter;
using LinCms.Base.Localizations;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base;

/// <summary>
/// 语言下的本地化资源
/// </summary>
[ApiExplorerSettings(GroupName = "base")]
[Area("base")]
[Route("api/base/resource")]
[ApiController]
public class ResourceController(IResourceService resourceService) : ControllerBase
{
    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除本地化资源", "本地化资源管理")]
    public Task DeleteAsync(long id)
    {
        return resourceService.DeleteAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ResourceDto>> GetListAsync([FromQuery] ResourceSearchDto searchDto)
    {
        return resourceService.GetListAsync(searchDto);
    }

    [HttpGet("{id}")]
    public Task<ResourceDto> GetAsync(long id)
    {
        return resourceService.GetAsync(id);
    }

    [HttpPost]
    [LinCmsAuthorize("创建本地化资源 ", "本地化资源管理")]
    public Task CreateAsync([FromBody] ResourceDto createResource)
    {
        return resourceService.CreateAsync(createResource);
    }

    [HttpPut]
    [LinCmsAuthorize("更新本地化资源 ", "本地化资源管理")]
    public Task UpdateAsync([FromBody] ResourceDto updateResource)
    {
        return resourceService.UpdateAsync(updateResource);
    }
}