using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Filter;
using LinCms.Base.Docs;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base;

/// <summary>
/// 简单文档示例，仅仅是一个demo
/// </summary>
[ApiExplorerSettings(GroupName = "base")]
[Route("api/base/docs")]
[ApiController]
public class DocController(IDocService docService) : ControllerBase
{
    [LinCmsAuthorize("删除文档", "文档管理")]
    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteAsync(long id)
    {
        await docService.DeleteAsync(id);
        return UnifyResponseDto.Success();

    }

    [LinCmsAuthorize("所有文档", "文档管理")]
    [HttpGet]
    public Task<PagedResultDto<DocDto>> GetListAsync([FromQuery] PageDto pageDto)
    {
        return docService.GetListAsync(pageDto);
    }

    [HttpGet("{id}")]
    public Task<DocDto> GetAsync(long id)
    {
        return docService.GetAsync(id);
    }

    [LinCmsAuthorize("新增文档", "文档管理")]
    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateDocDto createDoc)
    {
        await docService.CreateAsync(createDoc);
        return UnifyResponseDto.Success("新增文档成功");
    }

    [LinCmsAuthorize("编辑文档", "文档管理")]
    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] CreateUpdateDocDto updateDoc)
    {
        await docService.UpdateAsync(id, updateDoc);
        return UnifyResponseDto.Success("编辑文档成功");
    }
}