using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Base;
using LinCms.Core.Exceptions;
using LinCms.Infrastructure.Repositories;
using LinCms.Application.Contracts.v1.BaseTypes;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/type")]
    [ApiController]
    public class BaseTypeController : ControllerBase
    {
        private readonly AuditBaseRepository<BaseType> _baseTypeRepository;
        private readonly IMapper _mapper;
        public BaseTypeController(IMapper mapper, AuditBaseRepository<BaseType> baseTypeRepository)
        {
            _mapper = mapper;
            _baseTypeRepository = baseTypeRepository;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除字典类别", "字典类别")]
        public ResultDto DeleteBaseType(int id)
        {
            _baseTypeRepository.Delete(new BaseType { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<BaseTypeDto> Get()
        {
            List<BaseTypeDto> baseTypes = _baseTypeRepository.Select
                .OrderBy(r => r.SortCode)
                .OrderBy(r => r.Id)
                .ToList()
                .Select(r => _mapper.Map<BaseTypeDto>(r)).ToList();

            return baseTypes;
        }

        [HttpGet("{id}")]
        public BaseTypeDto Get(int id)
        {
            BaseType baseType = _baseTypeRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<BaseTypeDto>(baseType);
        }

        [LinCmsAuthorize("新增字典类别", "字典类别")]
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateBaseTypeDto createBaseType)
        {
            bool exist = _baseTypeRepository.Select.Any(r => r.TypeCode == createBaseType.TypeCode);
            if (exist)
            {
                throw new LinCmsException($"类别-编码[{createBaseType.TypeCode}]已存在");
            }

            BaseType baseType = _mapper.Map<BaseType>(createBaseType);
            _baseTypeRepository.Insert(baseType);
            return ResultDto.Success("新建类别成功");
        }

        [LinCmsAuthorize("编辑字典类别", "字典类别")]
        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] CreateUpdateBaseTypeDto updateBaseType)
        {
            BaseType baseType = _baseTypeRepository.Select.Where(r => r.Id == id).ToOne();
            if (baseType == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = _baseTypeRepository.Select.Any(r => r.TypeCode == updateBaseType.TypeCode && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"基础类别-编码[{updateBaseType.TypeCode}]已存在");
            }

            _mapper.Map(updateBaseType, baseType);

            _baseTypeRepository.Update(baseType);

            return ResultDto.Success("更新类别成功");
        }
    }
}