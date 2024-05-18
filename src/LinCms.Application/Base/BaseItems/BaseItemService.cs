using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Base;
using LinCms.Exceptions;

namespace LinCms.Base.BaseItems;

public class BaseItemService(IAuditBaseRepository<BaseItem> baseItemRepository,
        IAuditBaseRepository<BaseType> baseTypeRepository)
    : ApplicationService, IBaseItemService
{
    public async Task DeleteAsync(int id)
    {
        await baseItemRepository.DeleteAsync(new BaseItem { Id = id });
    }

    public async Task<List<BaseItemDto>> GetListAsync(string typeCode)
    {
        long baseTypeId = baseTypeRepository.Select.Where(r => r.TypeCode == typeCode).ToOne(r => r.Id);

        List<BaseItemDto> baseItems = (await baseItemRepository.Select
                .OrderBy(r => r.SortCode)
                .OrderBy(r => r.Id)
                .Where(r => r.BaseTypeId == baseTypeId)
                .ToListAsync())
            .Select(r => Mapper.Map<BaseItemDto>(r)).ToList();

        return baseItems;
    }

    public async Task<BaseItemDto> GetAsync(int id)
    {
        BaseItem baseItem = await baseItemRepository.Select.Where(a => a.Id == id).ToOneAsync();
        return Mapper.Map<BaseItemDto>(baseItem);
    }

    public async Task CreateAsync(CreateUpdateBaseItemDto createBaseItem)
    {
        bool exist = await baseItemRepository.Select.AnyAsync(r =>
            r.BaseTypeId == createBaseItem.BaseTypeId && r.ItemCode == createBaseItem.ItemCode);
        if (exist)
        {
            throw new LinCmsException($"编码[{createBaseItem.ItemCode}]已存在");
        }

        BaseItem baseItem = Mapper.Map<BaseItem>(createBaseItem);
        await baseItemRepository.InsertAsync(baseItem);
    }

    public async Task UpdateAsync(int id, CreateUpdateBaseItemDto updateBaseItem)
    {
        BaseItem baseItem = await baseItemRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (baseItem == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        bool typeExist = await baseTypeRepository.Select.AnyAsync(r => r.Id == updateBaseItem.BaseTypeId);
        if (!typeExist)
        {
            throw new LinCmsException("请选择正确的类别");
        }

        bool exist = await baseItemRepository.Select.AnyAsync(r =>
            r.BaseTypeId == updateBaseItem.BaseTypeId && r.ItemCode == updateBaseItem.ItemCode && r.Id != id);

        if (exist)
        {
            throw new LinCmsException($"编码[{updateBaseItem.ItemCode}]已存在");
        }

        Mapper.Map(updateBaseItem, destination: baseItem);
        await baseItemRepository.UpdateAsync(baseItem);
    }
}