using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Contracts.Base.BaseTypes;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Base;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Base
{
    [Route("v1/type")]
    [ApiController]
    public class BaseTypeController : ControllerBase
    {
        private readonly IAuditBaseRepository<BaseType> _baseTypeRepository;
        private readonly IMapper _mapper;
        public BaseTypeController(IMapper mapper, IAuditBaseRepository<BaseType> baseTypeRepository)
        {
            _mapper = mapper;
            _baseTypeRepository = baseTypeRepository;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除字典类别", "字典类别")]
        public UnifyResponseDto DeleteBaseType(int id)
        {
            _baseTypeRepository.Delete(new BaseType { Id = id });
            return UnifyResponseDto.Success();
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
        public UnifyResponseDto Create([FromBody] CreateUpdateBaseTypeDto createBaseType)
        {
            bool exist = _baseTypeRepository.Select.Any(r => r.TypeCode == createBaseType.TypeCode);
            if (exist)
            {
                throw new LinCmsException($"类别-编码[{createBaseType.TypeCode}]已存在");
            }

            BaseType baseType = _mapper.Map<BaseType>(createBaseType);
            _baseTypeRepository.Insert(baseType);
            return UnifyResponseDto.Success("新建类别成功");
        }

        [LinCmsAuthorize("编辑字典类别", "字典类别")]
        [HttpPut("{id}")]
        public UnifyResponseDto Update(int id, [FromBody] CreateUpdateBaseTypeDto updateBaseType)
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

            return UnifyResponseDto.Success("更新类别成功");
        }
    }
}