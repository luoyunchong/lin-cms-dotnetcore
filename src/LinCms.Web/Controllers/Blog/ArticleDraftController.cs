using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using LinCms.Application.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Application.Contracts.Blog.ArticleDrafts.Dtos;
using LinCms.Application.Contracts.Blog.ArticleDrafts;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/article/draft")]
    [ApiController]
    [Authorize]
    public class ArticleDraftController : ControllerBase
    {
        private readonly IArticleDraftService _articleDraftService;
        public ArticleDraftController(IArticleDraftService articleDraftService)
        {
            _articleDraftService = articleDraftService;
        }

        [HttpPut("{id}")]
        public async Task UpdateAsync(Guid id, [FromBody] UpdateArticleDraftDto updateArticleDto)
        {
            await _articleDraftService.UpdateAsync(id, updateArticleDto);
        }

        /// <summary>
        /// 用户的随笔草稿详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Task<ArticleDraftDto> GetAsync(Guid id)
        {
            return _articleDraftService.GetAsync(id);
        }
    }
}