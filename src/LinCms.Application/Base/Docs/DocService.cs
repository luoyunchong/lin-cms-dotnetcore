using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities.Base;
using LinCms.Extensions;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Base.Docs
{
    public class DocService :ApplicationService, IDocService
    {
        private readonly IAuditBaseRepository<Doc, long> Repository;

        public DocService(IAuditBaseRepository<Doc, long> repository)
        {
            Repository = repository;
        }

        public async Task DeleteAsync(long id)
        {
            await Repository.DeleteAsync(new Doc {Id = id});
        }

        public async Task<PagedResultDto<DocDto>> GetListAsync(PageDto pageDto)
        {
            List<DocDto> docss = (await Repository.Select
                        .ToPagerListAsync(pageDto, out long count)).Select(r => Mapper.Map<DocDto>(r)).ToList();

            return new PagedResultDto<DocDto>(docss, count);
        }

        public async Task<DocDto> GetAsync(long id)
        {
            Doc doc = await Repository.Select.Where(a => a.Id == id).ToOneAsync();
            return Mapper.Map<DocDto>(doc);
        }

        public async Task CreateAsync(CreateUpdateDocDto createDoc)
        {
            Doc doc = Mapper.Map<Doc>(createDoc);
            await Repository.InsertAsync(doc);
        }

        public async Task UpdateAsync(long id, CreateUpdateDocDto updateDoc)
        {
            Doc doc = await Repository.Select.Where(r => r.Id == id).ToOneAsync();
            if (doc == null)
            {
                throw new LinCmsException("该数据不存在");
            }
            Mapper.Map(updateDoc, doc);
            await Repository.UpdateAsync(doc);
        }
    }
}