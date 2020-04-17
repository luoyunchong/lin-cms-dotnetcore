using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<List<BaseTypeDto>> GetListAsync()
        {
            return await _baseTypeService.GetListAsync();
        }

        [HttpGet("{id}")]
        public async Task<BaseTypeDto> GetAsync(int id)
        {
            return await _baseTypeService.GetAsync(id);
        }

        [LinCmsAuthorize("新增字典类别", "字典类别")]
        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateBaseTypeDto createBaseType)
        {
             await _baseTypeService.CreateAsync(createBaseType);
            return UnifyResponseDto.Success("新建类别成功");
        }

        [LinCmsAuthorize("编辑字典类别", "字典类别")]
        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(int id, [FromBody] CreateUpdateBaseTypeDto updateBaseType)
        {
            await _baseTypeService.UpdateAsync(id,updateBaseType);
            return UnifyResponseDto.Success("更新类别成功");
        }
    }
}