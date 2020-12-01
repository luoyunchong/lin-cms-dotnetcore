using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.v1.Books
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