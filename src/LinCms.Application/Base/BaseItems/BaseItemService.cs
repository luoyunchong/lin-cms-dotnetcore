using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Base.BaseItems;
using LinCms.Application.Contracts.Base.BaseItems.Dtos;
using LinCms.Core.Entities.Base;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Base.BaseItems
{
    public class BaseItemService : IBaseItemService
    {
        private readonly IAuditBaseRepository<BaseItem> _baseItemRepository;
        private readonly IAuditBaseRepository<BaseType> _baseTypeRepository;
        private readonly IMapper _mapper;

        public BaseItemService(IAuditBaseRepository<BaseItem> baseItemRepository, IMapper mapper,
            IAuditBaseRepository<BaseType> baseTypeRepository)
        {
            _baseItemRepository = baseItemRepository;
            _mapper = mapper;
            _baseTypeRepository = baseTypeRepository;
        }

        public async Task DeleteAsync(int id)
        {
            await _baseItemRepository.DeleteAsync(new BaseItem {Id = id});
        }

        public async Task<List<BaseItemDto>> GetListAsync(string typeCode)
        {
            long baseTypeId = _baseTypeRepository.Select.Where(r => r.TypeCode == typeCode).ToOne(r => r.Id);

            List<BaseItemDto> baseItems = (await _baseItemRepository.Select
                    .OrderBy(r => r.SortCode)
                    .OrderBy(r => r.Id)
                    .Where(r => r.BaseTypeId == baseTypeId)
                    .ToListAsync())
                    .Select(r => _mapper.Map<BaseItemDto>(r)).ToList();

            return baseItems;
        }

        public async Task<BaseItemDto> GetAsync(int id)
        {
            BaseItem baseItem = await _baseItemRepository.Select.Where(a => a.Id == id).ToOneAsync();
            return _mapper.Map<BaseItemDto>(baseItem);
        }

        public async Task CreateAsync(CreateUpdateBaseItemDto createBaseItem)
        {
            bool exist = await _baseItemRepository.Select.AnyAsync(r =>
                r.BaseTypeId == createBaseItem.BaseTypeId && r.ItemCode == createBaseItem.ItemCode);
            if (exist)
            {
                throw new LinCmsException($"编码[{createBaseItem.ItemCode}]已存在");
            }

            BaseItem baseItem = _mapper.Map<BaseItem>(createBaseItem);
            await _baseItemRepository.InsertAsync(baseItem);
        }

        public async Task UpdateAsync(int id, CreateUpdateBaseItemDto updateBaseItem)
        {
            BaseItem baseItem = await _baseItemRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (baseItem == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool typeExist = await _baseTypeRepository.Select.AnyAsync(r => r.Id == updateBaseItem.BaseTypeId);
            if (!typeExist)
            {
                throw new LinCmsException("请选择正确的类别");
            }

            bool exist = await _baseItemRepository.Select.AnyAsync(r =>
                r.BaseTypeId == updateBaseItem.BaseTypeId && r.ItemCode == updateBaseItem.ItemCode && r.Id != id);

            if (exist)
            {
                throw new LinCmsException($"编码[{updateBaseItem.ItemCode}]已存在");
            }

            _mapper.Map(updateBaseItem, baseItem);
            await _baseItemRepository.UpdateAsync(baseItem);
        }
    }
}