using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.v1.Books.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Cms.Books
{
    public interface IBookService
    {

        Task<PagedResultDto<BookDto>> GetListAsync(PageDto pageDto);

        Task<BookDto> GetAsync(long id);

        Task CreateAsync(CreateUpdateBookDto inputDto);

        Task UpdateAsync(long id, CreateUpdateBookDto inputDto);

        Task DeleteAsync(long id);
    }
}