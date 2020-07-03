using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Base.Localizations;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Web.Data.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Area("base")]
    [Route("api/base/resource")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly ILocalResourceService _resourceService;
        public ResourceController(ILocalResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpDelete("{id}")]
        public async Task<UnifyResponseDto> DeleteAsync(long id)
        {
            await _resourceService.DeleteAsync(id);
            return UnifyResponseDto.Success();
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
        public UnifyResponseDto CreateAsync([FromBody] ResourceDto createResource)
        {
            _resourceService.CreateAsync(createResource);
            return UnifyResponseDto.Success("新建成功");
        }

        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync( [FromBody] ResourceDto updateResource)
        {
            await _resourceService.UpdateAsync(updateResource);
            return UnifyResponseDto.Success("更新成功");
        }
    }
}