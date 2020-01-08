using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Infrastructure.Repositories;
using LinCms.Plugins.Poem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Plugins.Poem.Controllers
{
    [Route("v1/poem")]
    [ApiController]
    [Authorize]
    public class PoemController : ControllerBase
    {
        private readonly AuditBaseRepository<LinPoem> _poemRepository;
        private readonly IMapper _mapper;
        public PoemController(AuditBaseRepository<LinPoem> poemRepository, IMapper mapper)
        {
            _poemRepository = poemRepository;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除诗词", "诗词")]
        public ResultDto DeletePoem(long id)
        {
            _poemRepository.Delete(new LinPoem { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<PoemDto> Get()
        {
            return _poemRepository.Select.OrderByDescending(r => r.Id).ToList()
                .Select(r => _mapper.Map<PoemDto>(r))
                .ToList();
        }

        [HttpGet("{id}")]
        public PoemDto Get(long id)
        {
            LinPoem poem = _poemRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<PoemDto>(poem);
        }

        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdatePoemDto createPoem)
        {
            LinPoem poem = _mapper.Map<LinPoem>(createPoem);
            _poemRepository.Insert(poem);
            return ResultDto.Success("新建诗词成功");
        }

        [HttpPut("{id}")]
        public ResultDto Put(long id, [FromBody] CreateUpdatePoemDto updatePoem)
        {
            LinPoem poem = _poemRepository.Select.Where(r => r.Id == id).ToOne();
            if (poem == null)
            {
                throw new LinCmsException("没有找到诗词");
            }

            //使用AutoMapper方法简化类与类之间的转换过程
            _mapper.Map(updatePoem, poem);

            _poemRepository.Update(poem);

            return ResultDto.Success("更新诗词成功");
        }
    }
}
