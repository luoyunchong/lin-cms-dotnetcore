using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.v1.BaseItems;
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
        [LinCmsAuthorize("删除基础资料", "基础资料")]
        public ResultDto DeleteBaseItem(int id)
        {
            _baseItemRepository.Delete(new BaseItem { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public List<BaseItemDto> Get([FromQuery]string typeCode)
        {
            int baseTypeId = _baseTypeRepository.Select.Where(r => r.TypeCode == typeCode).ToOne(r => r.Id);

            var baseItems = _baseItemRepository.Select.OrderByDescending(r => r.SortCode)
                .OrderBy(r => r.Id)
                .Where(r => r.BaseTypeId == baseTypeId)
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
        public ResultDto Post([FromBody] CreateUpdateBaseItemDto createBaseItem)
        {
            bool exist = _baseItemRepository.Select.Any(r => r.BaseTypeId == createBaseItem.BaseTypeId && r.ItemCode == createBaseItem.ItemCode);
            if (exist)
            {
                throw new LinCmsException($"基础资料-编码[{createBaseItem.ItemCode}]已存在");
            }

            BaseItem baseItem = _mapper.Map<BaseItem>(createBaseItem);
            _baseItemRepository.Insert(baseItem);
            return ResultDto.Success("新建基础资料成功");
        }

        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] CreateUpdateBaseItemDto updateBaseItem)
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
                throw new LinCmsException($"基础资料-编码[{updateBaseItem.ItemCode}]已存在");
            }

            _mapper.Map(updateBaseItem, baseItem);

            _baseItemRepository.Update(baseItem);

            return ResultDto.Success("更新基础资料成功");
        }
    }
}