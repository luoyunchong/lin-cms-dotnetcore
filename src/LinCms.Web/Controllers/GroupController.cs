using LinCms.Web.Models.Groups;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        public GroupController(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        [HttpGet("all")]
        public IEnumerable<LinGroup> Get()
        {
           return _freeSql.Select<LinGroup>().ToList();
        }

        [HttpGet("{id}")]
        public LinGroup Get(int id)
        {
            return _freeSql.Select<LinGroup>().Where(r=>r.Id==id).First();
        }

        [HttpPost]
        public void Post([FromBody] CreateGroupDto inputDto)
        {

        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            _freeSql.Update<LinGroup>(id).Set(a => new LinGroup()
            {
                Info = updateGroupDto.Info,
                Name = updateGroupDto.Name
            }).ExecuteAffrows();
        }

        [HttpDelete("{id}")]
        public ResultDto Delete(int id)
        {
            if(!_freeSql.Select<LinGroup>(new { id = id }).Any())
            {
                return ResultDto.Error("分组不存在，删除失败");
            }

            _freeSql.Delete<LinGroup>(new { id = id }).ExecuteAffrows();
            return ResultDto.Success("删除分组成功");
        }

     
    }
}
