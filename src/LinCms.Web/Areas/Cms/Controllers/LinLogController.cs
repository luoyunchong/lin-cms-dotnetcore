using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Domain;
using LinCms.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Areas.Cms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinLogController : ControllerBase
    {
        // GET: api/LinLog
        private readonly LinLogRepository _linLogRepository;

        public LinLogController(LinLogRepository linLogRepository)
        {
            _linLogRepository = linLogRepository;
        }

        [HttpGet]
        public IEnumerable<LinLog> Get()
        {
            return _linLogRepository.GetLogUsers();
        }

        // POST: api/LinLog
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/LinLog/5
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
