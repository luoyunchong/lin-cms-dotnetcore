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
        public void Delete(Comment comment)
        {
            int affrows = 0;
            //如果是根评论，删除所有的子评论
            if (!comment.RootCommentId.HasValue)
            {
                affrows += _commentAuditBaseRepository.Delete(r => r.RootCommentId == comment.Id);
            }
            else
            {
                _commentAuditBaseRepository.UpdateDiy.Set(r => r.ChildsCount - 1).Where(r => r.Id == comment.RootCommentId).ExecuteAffrows();
            }
         
            affrows += _commentAuditBaseRepository.Delete(new Comment { Id = comment.Id });

            switch (comment.SubjectType)
            {
                case 1:
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity - affrows).Where(r => r.Id == comment.SubjectId).ExecuteAffrows();
                    break;
            }
        }
    }
}
