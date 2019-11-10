using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.v1.MessageBoards;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{
    public class MessageBoardProfile:Profile
    {
        public MessageBoardProfile()
        {
            CreateMap<CreateMessageBoardDto, MessageBoard>();
            CreateMap<MessageBoard, MessageBoardDto>();
        }
    }
}
