using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Base;
using LinCms.Exceptions;

namespace LinCms.Base.BaseTypes;

/// <summary>
/// 数据字典-分类服务
/// </summary>
public class BaseTypeService : ApplicationService, IBaseTypeService
{
    private readonly IAuditBaseRepository<BaseType, long> _baseTypeRepository;

    public BaseTypeService(IAuditBaseRepository<BaseType, long> baseTypeRepository)
    {
        _baseTypeRepository = baseTypeRepository;
    }

    public Task DeleteAsync(long id)
    {
        return _baseTypeRepository.DeleteAsync(id);
    }

    public async Task<List<BaseTypeDto>> GetListAsync()
    {
        List<BaseTypeDto> baseTypes = (await _baseTypeRepository.Select
                .OrderBy(r => r.SortCode)
                .OrderBy(r => r.Id)
                .ToListAsync())
            .Select(r => Mapper.Map<BaseTypeDto>(r)).ToList();

        return baseTypes;
    }

    public async Task<BaseTypeDto> GetAsync(int id)
    {
        BaseType baseType = await _baseTypeRepository.Select.Where(a => a.Id == id).ToOneAsync();
        return Mapper.Map<BaseTypeDto>(baseType);
    }

    public async Task CreateAsync(CreateUpdateBaseTypeDto createBaseType)
    {
        bool exist = await _baseTypeRepository.Select.AnyAsync(r => r.TypeCode == createBaseType.TypeCode);
        if (exist)
        {
            throw new LinCmsException($"类别-编码[{createBaseType.TypeCode}]已存在");
        }

        BaseType baseType = Mapper.Map<BaseType>(createBaseType);
        await _baseTypeRepository.InsertAsync(baseType);
    }

    public async Task UpdateAsync(int id, CreateUpdateBaseTypeDto updateBaseType)
    {
        BaseType baseType = await _baseTypeRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (baseType == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        bool exist = await _baseTypeRepository.Select.AnyAsync(r => r.TypeCode == updateBaseType.TypeCode && r.Id != id);
        if (exist)
        {
            throw new LinCmsException($"基础类别-编码[{updateBaseType.TypeCode}]已存在");
        }

        Mapper.Map(updateBaseType, baseType);
        await _baseTypeRepository.UpdateAsync(baseType);
    }
}