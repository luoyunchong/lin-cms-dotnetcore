using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Blog.Comments
{
    public class CommentService : ICommentService
    {
        private readonly IAuditBaseRepository<Comment> _commentAuditBaseRepository;
        private readonly IAuditBaseRepository<Article> _articleRepository;
        public CommentService(IAuditBaseRepository<Comment> commentAuditBaseRepository, IAuditBaseRepository<Article> articleRepository)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
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
                affrows += await _commentAuditBaseRepository.DeleteAsync(r => r.RootCommentId == comment.Id);
            }
            else
            {
                await _commentAuditBaseRepository.UpdateDiy.Set(r => r.ChildsCount - 1).Where(r => r.Id == comment.RootCommentId).ExecuteAffrowsAsync();
            }

            affrows += await _commentAuditBaseRepository.DeleteAsync(new Comment { Id = comment.Id });

            switch (comment.SubjectType)
            {
                case 1:
                    await _articleRepository.UpdateDiy.Set(r => r.CommentQuantity - affrows).Where(r => r.Id == comment.SubjectId).ExecuteAffrowsAsync();
                    break;
            }
        }
    }
}
