﻿using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
    [Table(Name = "blog_user_tag")]
    public class UserTag : Entity<Guid>,ICreateAduitEntity
    {
        public Guid TagId { get; set; }
        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }
        [Navigate("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
