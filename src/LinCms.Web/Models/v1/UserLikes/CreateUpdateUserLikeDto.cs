using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.v1.UserLikes
{
    public class CreateUpdateUserLikeDto
    {
        public Guid SubjectId { get; set; }
        /// <summary>
        /// 1.文章 2 评论
        /// </summary>
        public int SubjectType { get; set; }
    }
}
