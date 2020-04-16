using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Blog.MessageBoards
{
    public class MessageBoardDto:EntityDto<Guid>
    {
        public string Text { get; set; }

        public string Ip { get; set; }
        public string Agent { get; set; }
        public string System { get; set; }
       
        public string GeoPosition { get; set; }
    
        public string UserHost { get; set; }
        public string Avatar { get; set; }
        public bool? IsAudit { get; set; }
    }
}
