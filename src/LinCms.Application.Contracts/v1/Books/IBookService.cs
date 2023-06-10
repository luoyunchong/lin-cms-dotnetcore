using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Data;

namespace LinCms.v1.Books;

public interface IBookService
{
    Task<List<BookDto>> GetListAsync();
    Task<PagedResultDto<BookDto>> GetPageListAsync(PageDto pageDto);

    Task<BookDto> GetAsync(long id);

    Task CreateAsync(CreateUpdateBookDto inputDto);

    Task UpdateAsync(long id, CreateUpdateBookDto inputDto);

    Task DeleteAsync(long id);
}