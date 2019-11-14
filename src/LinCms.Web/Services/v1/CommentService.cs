using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;

namespace LinCms.Web.Services.v1
{
    public class CommentService: ICommentService
    {
        private readonly AuditBaseRepository<Comment> _commentAuditBaseRepository;
        private readonly AuditBaseRepository<Article> _articleRepository;
        public CommentService(AuditBaseRepository<Comment> commentAuditBaseRepository, AuditBaseRepository<Article> articleRepository)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _articleRepository = articleRepository;
        }
        public void Delete(Comment comment)
        {

            //如果是根评论，删除所有的子评论
            if (!comment.RootCommentId.HasValue)
            {
                _commentAuditBaseRepository.Delete(r => r.RootCommentId == comment.Id);
            }
            else
            {
                _commentAuditBaseRepository.UpdateDiy.Set(r => r.ChildsCount - 1).Where(r => r.Id == comment.RootCommentId).ExecuteAffrows();
            }
            _commentAuditBaseRepository.Delete(new Comment { Id = comment.Id });

            switch (comment.SubjectType)
            {
                case 1:
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity - 1).Where(r => r.Id == comment.SubjectId).ExecuteAffrows();
                    break;
            }
        }
    }
}
