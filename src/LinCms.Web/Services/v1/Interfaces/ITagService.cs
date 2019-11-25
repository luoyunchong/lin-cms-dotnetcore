using LinCms.Web.Models.v1.Tags;
using LinCms.Zero.Data;

namespace LinCms.Web.Services.v1.Interfaces
{
    public interface ITagService
    {
         PagedResultDto<TagDto> Get(TagSearchDto searchDto);
    }
}
