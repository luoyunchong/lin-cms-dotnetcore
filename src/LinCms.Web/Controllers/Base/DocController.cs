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
public class DocController : ControllerBase
{
    private readonly IDocService _docService;
    public DocController(IDocService docService)
    {
        _docService = docService;
    }

    [LinCmsAuthorize("删除文档", "文档管理")]
    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteAsync(long id)
    {
        await _docService.DeleteAsync(id);
        return UnifyResponseDto.Success();

    }

    [LinCmsAuthorize("所有文档", "文档管理")]
    [HttpGet]
    public Task<PagedResultDto<DocDto>> GetListAsync([FromQuery] PageDto pageDto)
    {
        return _docService.GetListAsync(pageDto);
    }

    [HttpGet("{id}")]
    public Task<DocDto> GetAsync(long id)
    {
        return _docService.GetAsync(id);
    }

    [LinCmsAuthorize("新增文档", "文档管理")]
    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateDocDto createDoc)
    {
        await _docService.CreateAsync(createDoc);
        return UnifyResponseDto.Success("新增文档成功");
    }

    [LinCmsAuthorize("编辑文档", "文档管理")]
    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] CreateUpdateDocDto updateDoc)
    {
        await _docService.UpdateAsync(id, updateDoc);
        return UnifyResponseDto.Success("编辑文档成功");
    }
}