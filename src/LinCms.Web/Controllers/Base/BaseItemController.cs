using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Contracts.Base.BaseItems;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Base;
using LinCms.Core.Exceptions;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Base
{
    [Route("v1/item")]
    [ApiController]
    public class BaseItemController : ControllerBase
    {
        private readonly AuditBaseRepository<BaseItem> _baseItemRepository;
        private readonly AuditBaseRepository<BaseType> _baseTypeRepository;
        private readonly IMapper _mapper;
        public BaseItemController(AuditBaseRepository<BaseItem> baseItemRepository, IMapper mapper, AuditBaseRepository<BaseType> baseTypeRepository)
        {
            _baseItemRepository = baseItemRepository;
            _mapper = mapper;
            _baseTypeRepository = baseTypeRepository;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除字典", "字典管理")]
        public UnifyResponseDto DeleteBaseItem(int id)
        {
            _baseItemRepository.Delete(new BaseItem { Id = id });
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public List<BaseItemDto> Get([FromQuery]string typeCode)
        {
            long baseTypeId =_baseTypeRepository.Select.Where(r => r.TypeCode == typeCode).ToOne(r => r.Id);

            List<BaseItemDto> baseItems = _baseItemRepository.Select
                .OrderBy(r => r.SortCode)
                .OrderBy(r => r.Id)
                .Where( r => r.BaseTypeId == baseTypeId)
                .ToList()
                .Select(r => _mapper.Map<BaseItemDto>(r)).ToList();

            return baseItems;
        }

        [HttpGet("{id}")]
        public BaseItemDto Get(int id)
        {
            BaseItem baseItem = _baseItemRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<BaseItemDto>(baseItem);
        }

        [HttpPost]
        [LinCmsAuthorize("新增字典", "字典管理")]
        public UnifyResponseDto Post([FromBody] CreateUpdateBaseItemDto createBaseItem)
        {
            bool exist = _baseItemRepository.Select.Any(r => r.BaseTypeId == createBaseItem.BaseTypeId && r.ItemCode == createBaseItem.ItemCode);
            if (exist)
            {
                throw new LinCmsException($"编码[{createBaseItem.ItemCode}]已存在");
            }

            BaseItem baseItem = _mapper.Map<BaseItem>(createBaseItem);
            _baseItemRepository.Insert(baseItem);
            return UnifyResponseDto.Success("新建字典成功");
        }

        [HttpPut("{id}")]
        [LinCmsAuthorize("编辑字典", "字典管理")]
        public UnifyResponseDto Put(int id, [FromBody] CreateUpdateBaseItemDto updateBaseItem)
        {
            BaseItem baseItem = _baseItemRepository.Select.Where(r => r.Id == id).ToOne();
            if (baseItem == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool typeExist= _baseTypeRepository.Select.Any(r => r.Id == updateBaseItem.BaseTypeId);

            if (!typeExist)
            {
                throw new LinCmsException("请选择正确的类别");
            }

            bool exist = _baseItemRepository.Select.Any(r => r.BaseTypeId == updateBaseItem.BaseTypeId && r.ItemCode == updateBaseItem.ItemCode && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"编码[{updateBaseItem.ItemCode}]已存在");
            }

            _mapper.Map(updateBaseItem, baseItem);

            _baseItemRepository.Update(baseItem);

            return UnifyResponseDto.Success("更新字典成功");
        }
    }
}