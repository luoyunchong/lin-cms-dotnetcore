﻿using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Blog.Notifications
{
    public class CommentEntry:EntityDto<Guid>
    {
        public Guid? RespId { get; set; }
        public string Text { get; set; }
    }
}
