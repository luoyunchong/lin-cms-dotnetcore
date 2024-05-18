using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Aop.Filter;
using LinCms.Data;
using LinCms.FreeSql;
using LinCms.v1.Books;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.v1;

/// <summary>
/// 图书
/// </summary>
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/book")]
[ApiController]
// [Authorize]
public class BookController(IBookService bookService) : ControllerBase
{
    [Logger("删除图书")]
    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除图书", "图书")]
    public async Task<UnifyResponseDto> DeleteAsync(int id)
    {
        await bookService.DeleteAsync(id);
        return UnifyResponseDto.Success();
    }

    [HttpGet("list")]
    public async Task<List<BookDto>> GetListAsync()
    {
        return await bookService.GetListAsync();
    }

    [HttpGet]
    public async Task<PagedResultDto<BookDto>> GetPageListAsync([FromQuery] PageDto pageDto)
    {
        return await bookService.GetPageListAsync(pageDto);
    }

    [HttpGet("{id}")]
    public async Task<BookDto> GetAsync(int id)
    {
        return await bookService.GetAsync(id);
    }

    [Logger("新建图书")]
    [HttpPost]
    [LinCmsAuthorize("新建图书", "图书")]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateBookDto createBook)
    {
        await bookService.CreateAsync(createBook);
        return UnifyResponseDto.Success("新建图书成功");
    }

    [Logger("更新图书")]
    [HttpPut("{id}")]
    [LinCmsAuthorize("更新图书", "图书")]
    public async Task<UnifyResponseDto> UpdateAsync(int id, [FromBody] CreateUpdateBookDto updateBook)
    {
        await bookService.UpdateAsync(id, updateBook);
        return UnifyResponseDto.Success("更新图书成功");
    }
}