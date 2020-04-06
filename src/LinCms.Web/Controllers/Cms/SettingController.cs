using System;
using System.Threading.Tasks;
using LinCms.Application.Cms.Settings;
using LinCms.Application.Contracts.Cms.Settings;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/setting")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingRepository _settingRepository;
        public SettingController(ISettingService settingService, ICurrentUser currentUser, ISettingRepository settingRepository)
        {
            _settingService = settingService;
            _currentUser = currentUser;
            _settingRepository = settingRepository;
        }

        [LinCmsAuthorize("得到所有设置", "设置")]
        [HttpGet]
        public PagedResultDto<SettingDto> GetPagedListAsync([FromQuery] PageDto pageDto)
        {
            return _settingService.GetPagedListAsync(pageDto);
        }

        [LinCmsAuthorize("删除设置", "设置")]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _settingRepository.DeleteAsync(id);
        }

        [LinCmsAuthorize("新增设置", "设置")]
        [HttpPost]
        public async Task PostAsync([FromBody]CreateUpdateSettingDto createSetting)
        {
            await _settingService.PostAsync(createSetting);
        }

        [LinCmsAuthorize("修改设置", "设置")]
        [HttpPut("{id}")]
        public async Task PutAsync(Guid id,[FromBody]CreateUpdateSettingDto updateSettingDto)
        {
            await _settingService.PutAsync(id, updateSettingDto);
        }

        [HttpGet("{id}")]
        public SettingDto Get(Guid id)
        {
            return _settingService.Get(id);
        }


        [HttpPost("editor")]
        public async Task SetEditorAsync(string value)
        {
            CreateUpdateSettingDto createSetting = new CreateUpdateSettingDto
            {
                Value = value,
                ProviderName = "U",
                ProviderKey = _currentUser.Id.ToString(),
                Name = "Article.Editor"
            };
            await _settingService.SetAsync(createSetting);
        }

        [HttpGet("editor")]
        public async Task<string> GetOrNullAsync()
        {
            string name = "Article.Editor";
            string providerName = "U";
            string providerKey = _currentUser.Id.ToString();

            return await _settingService.GetOrNullAsync(name, providerName, providerKey);
        }

    }
}
