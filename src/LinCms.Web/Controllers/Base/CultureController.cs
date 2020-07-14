using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Base.Cultures;
using LinCms.Application.Contracts.Base.Localizations;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Web.Data.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Area("base")]
    [Route("api/base/culture")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _cultureService;
        public CultureController(ICultureService cultureService)
        {
            _cultureService = cultureService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除本地化 ", "本地化语言")]
        public async Task<UnifyResponseDto> DeleteAsync(long id)
        {
            await _cultureService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public async Task<List<CultureDto>> GetListAsync()
        {
            return await _cultureService.GetListAsync();
        }

        [HttpGet("{id}")]
        public Task<CultureDto> GetAsync(long id)
        {
            return _cultureService.GetAsync(id);
        }

        [HttpPost]
        [LinCmsAuthorize("创建本地化", "本地化语言")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CultureDto createCulture)
        {
            await _cultureService.CreateAsync(createCulture);
            return UnifyResponseDto.Success("新建成功");
        }

        [HttpPut]
        [LinCmsAuthorize("更新本地化", "本地化语言")]
        public async Task<UnifyResponseDto> UpdateAsync([FromBody] CultureDto updateCulture)
        {
            await _cultureService.UpdateAsync(updateCulture);
            return UnifyResponseDto.Success("更新成功");
        }
    }
}