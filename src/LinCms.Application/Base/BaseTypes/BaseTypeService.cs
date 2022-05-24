using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Entities.Base;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Base.BaseTypes
{
    public class BaseTypeService : ApplicationService, IBaseTypeService
    {
        private readonly IAuditBaseRepository<BaseType> _baseTypeRepository;

        public BaseTypeService(IAuditBaseRepository<BaseType> baseTypeRepository)
        {
            _baseTypeRepository = baseTypeRepository;
        }

        public async Task DeleteAsync(int id)
        {
            await _baseTypeRepository.DeleteAsync(new BaseType { Id = id });
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
}