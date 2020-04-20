using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Blog.Comments
{
    public class CommentService : ICommentService
    {
        private readonly IAuditBaseRepository<Comment> _commentRepository;
        private readonly IAuditBaseRepository<Article> _articleRepository;
        public CommentService(IAuditBaseRepository<Comment> commentRepository, IAuditBaseRepository<Article> articleRepository)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
        }
        /// <summary>
        /// 删除评论并同步随笔数量
        /// </summary>
        /// <param name="comment"></param>
        public async Task DeleteAsync(Comment comment)
        {
            int affrows = 0;
            //如果是根评论，删除所有的子评论
            if (!comment.RootCommentId.HasValue)
            {
                affrows += await _commentRepository.DeleteAsync(r => r.RootCommentId == comment.Id);
            }
            else
            {
                await _commentRepository.UpdateDiy.Set(r => r.ChildsCount - 1).Where(r => r.Id == comment.RootCommentId).ExecuteAffrowsAsync();
            }

            affrows += await _commentRepository.DeleteAsync(new Comment { Id = comment.Id });

            switch (comment.SubjectType)
            {
                case 1:
                    await _articleRepository.UpdateDiy.Set(r => r.CommentQuantity - affrows).Where(r => r.Id == comment.SubjectId).ExecuteAffrowsAsync();
                    break;
            }
        }

        public async Task UpdateLikeQuantity(Guid subjectId, int likesQuantity)
        {
            Comment comment = _commentRepository.Select.Where(r => r.Id == subjectId).ToOne();
            if (comment.IsAudit == false)
            {
                throw new LinCmsException("该评论因违规被拉黑");
            }
            if (likesQuantity < 0)
            {
                if (comment.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }
            comment.LikesQuantity += likesQuantity;
            await _commentRepository.UpdateAsync(comment);
            //_commentRepository.UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }
    }
}
