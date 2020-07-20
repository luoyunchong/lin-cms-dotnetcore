using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Blog.Classifys;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog
{
    [Area("blog")]
    [Route("api/blog/classifies")]
    [ApiController]
    public class ClassifyController : ControllerBase
    {
        private readonly IClassifyService _classifyService;

        public ClassifyController(IClassifyService classifyService)
        {
            _classifyService = classifyService;
        }

        [HttpDelete("{id}")]
        public async Task<UnifyResponseDto> DeleteClassify(Guid id)
        {
            await _classifyService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public List<ClassifyDto> GetListByUserId(long? userId)
        {
            return _classifyService.GetListByUserId(userId);
        }

        [LinCmsAuthorize("删除", "分类专栏")]
        [HttpDelete("cms/{id}")]
        public async Task<UnifyResponseDto> Delete(Guid id)
        {
            await _classifyService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        [LinCmsAuthorize("分类专栏列表", "分类专栏")]
        [HttpGet("cms")]
        public async Task<PagedResultDto<ClassifyDto>> GetListAsync([FromQuery] ClassifySearchDto pageDto)
        {
            return await _classifyService.GetListAsync(pageDto);
        }

        [HttpGet("{id}")]
        public async Task<ClassifyDto> GetAsync(Guid id)
        {
            return await _classifyService.GetAsync(id);
        }

        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateClassifyDto createClassify)
        {
            await _classifyService.CreateAsync(createClassify);
            return UnifyResponseDto.Success("新建分类专栏成功");
        }

        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateClassifyDto updateClassify)
        {
            await _classifyService.UpdateAsync(id, updateClassify);
            return UnifyResponseDto.Success("更新分类专栏成功");
        }
    }
}