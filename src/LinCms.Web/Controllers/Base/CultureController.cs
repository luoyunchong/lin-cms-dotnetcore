using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Base.Localizations;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Base;

/// <summary>
/// 多语言
/// </summary>
[ApiExplorerSettings(GroupName = "base")]
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
    public Task DeleteAsync(long id)
    {
        return _cultureService.DeleteAsync(id);
    }

    [HttpGet]
    public Task<List<CultureDto>> GetListAsync()
    {
        return _cultureService.GetListAsync();
    }

    [HttpGet("{id}")]
    public Task<CultureDto> GetAsync(long id)
    {
        return _cultureService.GetAsync(id);
    }

    [HttpPost]
    [LinCmsAuthorize("创建本地化", "本地化语言")]
    public Task<CultureDto> CreateAsync([FromBody] CultureDto createCulture)
    {
        return _cultureService.CreateAsync(createCulture);
    }

    [HttpPut]
    [LinCmsAuthorize("更新本地化", "本地化语言")]
    public Task<CultureDto> UpdateAsync([FromBody] CultureDto updateCulture)
    {
        return _cultureService.UpdateAsync(updateCulture);
    }
}