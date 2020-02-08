﻿using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Users
{
    public class UserNoviceDto:EntityDto
    {
        public string Introduction { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}
