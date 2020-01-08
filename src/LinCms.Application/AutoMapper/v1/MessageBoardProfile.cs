using AutoMapper;
using LinCms.Application.Contracts.v1.MessageBoards;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
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
