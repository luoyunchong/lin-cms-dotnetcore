using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Data;
using LinCms.Entities.Base;
using LinCms.Exceptions;
using LinCms.Extensions;

namespace LinCms.Base.Docs;

public class DocService(IAuditBaseRepository<Doc, long> repository) : ApplicationService, IDocService
{
    public async Task DeleteAsync(long id)
    {
        await repository.DeleteAsync(new Doc { Id = id });
    }

    public async Task<PagedResultDto<DocDto>> GetListAsync(PageDto pageDto)
    {
        List<DocDto> docss = (await repository.Select
            .ToPagerListAsync(pageDto, out long count)).Select(r => Mapper.Map<DocDto>(r)).ToList();

        return new PagedResultDto<DocDto>(docss, count);
    }

    public async Task<DocDto> GetAsync(long id)
    {
        Doc doc = await repository.Select.Where(a => a.Id == id).ToOneAsync();
        return Mapper.Map<DocDto>(doc);
    }

    public async Task CreateAsync(CreateUpdateDocDto createDoc)
    {
        Doc doc = Mapper.Map<Doc>(createDoc);
        await repository.InsertAsync(doc);
    }

    public async Task UpdateAsync(long id, CreateUpdateDocDto updateDoc)
    {
        Doc doc = await repository.Select.Where(r => r.Id == id).ToOneAsync();
        if (doc == null)
        {
            throw new LinCmsException("该数据不存在");
        }
        Mapper.Map(updateDoc, doc);
        await repository.UpdateAsync(doc);
    }
}