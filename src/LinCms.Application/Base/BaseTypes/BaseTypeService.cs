using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Base.BaseTypes;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;
using LinCms.Core.Entities.Base;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Base.BaseTypes
{
    public class BaseTypeService:IBaseTypeService
    {
        private readonly IAuditBaseRepository<BaseType> _baseTypeRepository;
        private readonly IMapper _mapper;

        public BaseTypeService(IMapper mapper, IAuditBaseRepository<BaseType> baseTypeRepository)
        {
            _mapper = mapper;
            _baseTypeRepository = baseTypeRepository;
        }

        public async Task DeleteAsync(int id)
        {
            await _baseTypeRepository.DeleteAsync(new BaseType {Id = id});
        }

        public async Task<List<BaseTypeDto>> GetListAsync()
        {
            List<BaseTypeDto> baseTypes = (await _baseTypeRepository.Select
                    .OrderBy(r => r.SortCode)
                    .OrderBy(r => r.Id)
                    .ToListAsync())
                .Select(r => _mapper.Map<BaseTypeDto>(r)).ToList();

            return baseTypes;
        }

        public async Task<BaseTypeDto> GetAsync(int id)
        {
            BaseType baseType = await _baseTypeRepository.Select.Where(a => a.Id == id).ToOneAsync();
            return _mapper.Map<BaseTypeDto>(baseType);
        }

        public async Task CreateAsync(CreateUpdateBaseTypeDto createBaseType)
        {
            bool exist = await _baseTypeRepository.Select.AnyAsync(r => r.TypeCode == createBaseType.TypeCode);
            if (exist)
            {
                throw new LinCmsException($"类别-编码[{createBaseType.TypeCode}]已存在");
            }

            BaseType baseType = _mapper.Map<BaseType>(createBaseType);
            await _baseTypeRepository.InsertAsync(baseType);
        }

        public async Task UpdateAsync(int id, CreateUpdateBaseTypeDto updateBaseType)
        {
            BaseType baseType = await _baseTypeRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (baseType == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist =
                await _baseTypeRepository.Select.AnyAsync(r => r.TypeCode == updateBaseType.TypeCode && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"基础类别-编码[{updateBaseType.TypeCode}]已存在");
            }

            _mapper.Map(updateBaseType, baseType);
            await _baseTypeRepository.UpdateAsync(baseType);
        }
    }
}