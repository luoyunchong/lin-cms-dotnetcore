using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Web.Models.v1.Classifys;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/classify")]
    [ApiController]
    public class ClassifyController : ControllerBase
    {
        private readonly AuditBaseRepository<Classify> _classifyRepository;
        private readonly IMapper _mapper;
        public ClassifyController(AuditBaseRepository<Classify> classifyRepository, IMapper mapper)
        {
            _classifyRepository = classifyRepository;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除分类专栏", "分类专栏")]
        public ResultDto DeleteClassify(Guid id)
        {
            _classifyRepository.Delete(new Classify { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<ClassifyDto> Get()
        {
            List<ClassifyDto> classifys = _classifyRepository.Select.OrderBy(r=>r.SortCode)
                .OrderByDescending(r => r.Id)
                .ToList()
                .Select(r => _mapper.Map<ClassifyDto>(r)).ToList();

            return classifys;
        }

        [HttpGet("{id}")]
        public ClassifyDto Get(Guid id)
        {
            Classify classify = _classifyRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<ClassifyDto>(classify);
        }

        [HttpPost]
        [LinCmsAuthorize("新增分类专栏", "分类专栏")]
        public ResultDto Post([FromBody] CreateUpdateClassifyDto createClassify)
        {
            bool exist = _classifyRepository.Select.Any(r => r.ClassifyName==createClassify.ClassifyName);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{createClassify.ClassifyName}]已存在");
            }
            bool existCode= _classifyRepository.Select.Any(r => r.ClassifyCode == createClassify.ClassifyCode);
            if (existCode)
            {
                throw new LinCmsException($"分类专栏[{createClassify.ClassifyCode}]已存在");
            }

            Classify classify = _mapper.Map<Classify>(createClassify);
            _classifyRepository.Insert(classify);
            return ResultDto.Success("新建分类专栏成功");
        }

        [HttpPut("{id}")]
        [LinCmsAuthorize("编辑分类专栏", "分类专栏")]
        public ResultDto Put(Guid id, [FromBody] CreateUpdateClassifyDto updateClassify)
        {
            Classify classify = _classifyRepository.Select.Where(r => r.Id == id).ToOne();
            if (classify == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = _classifyRepository.Select.Any(r => r.ClassifyName == updateClassify.ClassifyName && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{updateClassify.ClassifyName}]已存在");
            }

            bool exist2 = _classifyRepository.Select.Any(r => r.ClassifyCode == updateClassify.ClassifyCode && r.Id != id);
            if (exist2)
            {
                throw new LinCmsException($"分类专栏[{updateClassify.ClassifyCode}]已存在");
            }

            _mapper.Map(updateClassify, classify);

            _classifyRepository.Update(classify);

            return ResultDto.Success("更新分类专栏成功");
        }

    }
}