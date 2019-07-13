using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers
{
    [Route("cms/group")]
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

        [HttpGet("{id}", Name = "Get")]
        public LinGroup Get(int id)
        {
            return _freeSql.Select<LinGroup>().Where(r=>r.Id==id).First();
        }

        // POST: api/Group
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Group/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
