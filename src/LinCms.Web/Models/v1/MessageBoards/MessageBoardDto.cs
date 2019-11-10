using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.MessageBoards
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
        public bool? IsAudited { get; set; }
    }
}
