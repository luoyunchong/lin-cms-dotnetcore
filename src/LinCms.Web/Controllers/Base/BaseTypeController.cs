using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Base.BaseTypes;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base
{
    /// <summary>
    /// 数据字典-分类
    /// </summary>
    [ApiExplorerSettings(GroupName = "base")]
    [Area("base")]
    [Route("api/base/type")]
    [ApiController]
    public class BaseTypeController : ControllerBase
    {
        private readonly IBaseTypeService _baseTypeService;
        public BaseTypeController(IBaseTypeService baseTypeService)
        {
            _baseTypeService = baseTypeService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除字典类别", "字典类别")]
        public async Task<UnifyResponseDto> DeleteAsync(int id)
        {
            await _baseTypeService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public Task<List<BaseTypeDto>> GetListAsync()
        {
            return _baseTypeService.GetListAsync();
        }

        [HttpGet("{id}")]
        public Task<BaseTypeDto> GetAsync(int id)
        {
            return _baseTypeService.GetAsync(id);
        }

        [HttpPost]
        [LinCmsAuthorize("新增字典类别", "字典类别")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateBaseTypeDto createBaseType)
        {
            await _baseTypeService.CreateAsync(createBaseType);
            return UnifyResponseDto.Success("新建类别成功");
        }

        [HttpPut("{id}")]
        [LinCmsAuthorize("编辑字典类别", "字典类别")]
        public async Task<UnifyResponseDto> UpdateAsync(int id, [FromBody] CreateUpdateBaseTypeDto updateBaseType)
        {
            await _baseTypeService.UpdateAsync(id, updateBaseType);
            return UnifyResponseDto.Success("更新类别成功");
        }
    }
}