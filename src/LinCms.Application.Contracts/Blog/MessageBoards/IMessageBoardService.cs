using System;
using System.Threading.Tasks;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.MessageBoards
{
    public interface IMessageBoardService
    {
        Task<PagedResultDto<MessageBoardDto>> GetListAsync(PageDto pageDto);

        Task CreateAsync(CreateMessageBoardDto createMessageBoardDto);

        Task UpdateAsync(Guid id, bool isAudit);
    }
}