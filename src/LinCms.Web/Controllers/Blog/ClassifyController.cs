using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Filter;
using LinCms.Blog.Classifys;
using LinCms.Data;

using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 分类专栏
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/classifies")]
[ApiController]
public class ClassifyController(IClassifyService classifyService) : ControllerBase
{
  
  
    public Task<PagedResultDto<ClassifyDto>> GetListAsync([FromQuery] ClassifySearchDto pageDto)
    {
        return classifyService.GetListAsync(pageDto);
    }

    [HttpGet("{id}")]
    public Task<ClassifyDto> GetAsync(Guid id)
    {
        return classifyService.GetAsync(id);
    }

    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateClassifyDto createClassify)
    {
        await classifyService.CreateAsync(createClassify);
        return UnifyResponseDto.Success("新建分类专栏成功");
    }

    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateClassifyDto updateClassify)
    {
        await classifyService.UpdateAsync(id, updateClassify);
        return UnifyResponseDto.Success("更新分类专栏成功");
    }

    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> Delete(Guid id)
    {
        await classifyService.DeleteAsync(id);
        return UnifyResponseDto.Success();
    }
}