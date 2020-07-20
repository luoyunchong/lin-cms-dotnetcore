using System;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Blog.ArticleDrafts;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Articles
{
    public class ArticleDraftService : IArticleDraftService
    {
        private readonly IAuditBaseRepository<ArticleDraft> _articleDraftRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        public ArticleDraftService(IAuditBaseRepository<ArticleDraft> articleDraftRepository, ICurrentUser currentUser, IMapper mapper)
        {
            _articleDraftRepository = articleDraftRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ArticleDraftDto> GetAsync(Guid id)
        {
            ArticleDraft articleDraft = await _articleDraftRepository.Where(r => r.Id == id && r.CreateUserId == _currentUser.Id).ToOneAsync();
            return _mapper.Map<ArticleDraftDto>(articleDraft);
        }

        public async Task UpdateAsync(Guid id, UpdateArticleDraftDto updateArticleDto)
        {
            ArticleDraft articleDraft = await _articleDraftRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (articleDraft != null && articleDraft.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权修改他人的随笔");
            }
            if(articleDraft==null)
            {
                articleDraft = new ArticleDraft { Id = id,CreateUserId = _currentUser.Id??0,CreateTime = DateTime.Now};
            }
            _mapper.Map(updateArticleDto, articleDraft);
            await _articleDraftRepository.InsertOrUpdateAsync(articleDraft);
        }
    }
}
