using LinCms.Zero.Authorization;
using LinCms.Zero.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除图书", "图书")]
        public ResultDto DeleteBook(int id)
        {
            return ResultDto.Success();
        }
    }
}