using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Base.Localizations;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base
{
    /// <summary>
    /// 语言下的本地化资源
    /// </summary>
    [ApiExplorerSettings(GroupName = "base")]
    [Area("base")]
    [Route("api/base/resource")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService _resourceService;
        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除本地化资源", "本地化资源管理")]
        public Task DeleteAsync(long id)
        {
            return _resourceService.DeleteAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<ResourceDto>> GetListAsync([FromQuery] ResourceSearchDto searchDto)
        {
            return _resourceService.GetListAsync(searchDto);
        }

        [HttpGet("{id}")]
        public Task<ResourceDto> GetAsync(long id)
        {
            return _resourceService.GetAsync(id);
        }

        [HttpPost]
        [LinCmsAuthorize("创建本地化资源 ", "本地化资源管理")]
        public Task CreateAsync([FromBody] ResourceDto createResource)
        {
            return _resourceService.CreateAsync(createResource);
        }

        [HttpPut]
        [LinCmsAuthorize("更新本地化资源 ", "本地化资源管理")]
        public Task UpdateAsync([FromBody] ResourceDto updateResource)
        {
            return _resourceService.UpdateAsync(updateResource);
        }
    }
}