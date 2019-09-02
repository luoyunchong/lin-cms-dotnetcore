using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.v1.BaseTypes;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Base;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Http;
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
        [LinCmsAuthorize("删除基础资料", "基础资料")]
        public ResultDto DeleteBaseType(int id)
        {
            _baseTypeRepository.Delete(new BaseType { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<BaseTypeDto> Get()
        {
            var baseTypes = _baseTypeRepository.Select.OrderByDescending(r => r.SortCode)
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

        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateBaseTypeDto createBaseType)
        {
            bool exist = _baseTypeRepository.Select.Any(r => r.TypeCode == createBaseType.TypeCode);
            if (exist)
            {
                throw new LinCmsException($"基础类别-编码[{createBaseType.TypeCode}]已存在");
            }

            BaseType baseType = _mapper.Map<BaseType>(createBaseType);
            _baseTypeRepository.Insert(baseType);
            return ResultDto.Success("新建基础类别成功");
        }

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

            return ResultDto.Success("更新基础类别成功");
        }
    }
}