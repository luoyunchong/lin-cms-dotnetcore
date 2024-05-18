using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Base.BaseItems;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base;

/// <summary>
/// 数据字典-详情项
/// </summary>
[ApiExplorerSettings(GroupName = "base")]
[Area("base")]
[Route("api/base/item")]
[ApiController]
public class BaseItemController(IBaseItemService baseItemService) : ControllerBase
{
    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除字典", "字典管理")]
    public async Task<UnifyResponseDto> DeleteAsync(int id)
    {
        await baseItemService.DeleteAsync(id);
        return UnifyResponseDto.Success();
    }

    [HttpGet]
    public Task<List<BaseItemDto>> GetListAsync([FromQuery] string typeCode)
    {
        return baseItemService.GetListAsync(typeCode); ;
    }

    [HttpGet("{id}")]
    public Task<BaseItemDto> GetAsync(int id)
    {
        return baseItemService.GetAsync(id);
    }

    [HttpPost]
    [LinCmsAuthorize("新增字典", "字典管理")]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateBaseItemDto createBaseItem)
    {
        await baseItemService.CreateAsync(createBaseItem);
        return UnifyResponseDto.Success("新建字典成功");
    }

    [HttpPut("{id}")]
    [LinCmsAuthorize("编辑字典", "字典管理")]
    public async Task<UnifyResponseDto> UpdateAsync(int id, [FromBody] CreateUpdateBaseItemDto updateBaseItem)
    {
        await baseItemService.UpdateAsync(id, updateBaseItem);
        return UnifyResponseDto.Success("更新字典成功");
    }
}