﻿using System;

namespace LinCms.Domain
{
    public class LinLog : ICreateAduitEntity
    {
        public string Message { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int StatusCode { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string Authority { get; set; }

        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
