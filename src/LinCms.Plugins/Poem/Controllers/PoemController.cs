using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
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
        private readonly IAuditBaseRepository<LinPoem> _poemRepository;
        private readonly IMapper _mapper;
        public PoemController(IAuditBaseRepository<LinPoem> poemRepository, IMapper mapper)
        {
            _poemRepository = poemRepository;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除诗词", "诗词")]
        public UnifyResponseDto DeletePoem(long id)
        {
            _poemRepository.Delete(new LinPoem { Id = id });
            return UnifyResponseDto.Success();
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
        public UnifyResponseDto Post([FromBody] CreateUpdatePoemDto createPoem)
        {
            LinPoem poem = _mapper.Map<LinPoem>(createPoem);
            _poemRepository.Insert(poem);
            return UnifyResponseDto.Success("新建诗词成功");
        }

        [HttpPut("{id}")]
        public UnifyResponseDto Put(long id, [FromBody] CreateUpdatePoemDto updatePoem)
        {
            LinPoem poem = _poemRepository.Select.Where(r => r.Id == id).ToOne();
            if (poem == null)
            {
                throw new LinCmsException("没有找到诗词");
            }

            _mapper.Map(updatePoem, poem);
            _poemRepository.Update(poem);
            return UnifyResponseDto.Success("更新诗词成功");
        }
    }
}
