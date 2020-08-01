using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.Base.Docs
{
    public interface IDocService
    {
        Task DeleteAsync(long id);

        Task<PagedResultDto<DocDto>> GetListAsync(PageDto pageDto);

        Task<DocDto> GetAsync(long id);

        Task CreateAsync(CreateUpdateDocDto createDoc);

        Task UpdateAsync(long id, CreateUpdateDocDto updateDoc);
    }
}