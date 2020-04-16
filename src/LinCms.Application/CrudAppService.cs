using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Application.Contracts;
using LinCms.Application.Contracts.Blog.Channels.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Application
{
    public abstract class CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput,
            TUpdateInput>
        : ApplicationService,ICrudAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        protected  IAuditBaseRepository<TEntity, TKey> Repository { get; }

        protected CrudAppService(IAuditBaseRepository<TEntity, TKey>repository)
        {
            Repository = repository;
        }

        protected virtual ISelect<TEntity> CreateFilteredQuery(TGetListInput input)
        {
            return Repository.Select;
        }

        protected virtual TGetListOutputDto MapToGetListOutputDto(TEntity entity)
        {
            return Mapper.Map<TEntity, TGetListOutputDto>(entity);
        }

        protected virtual ISelect<TEntity> ApplyPaging(ISelect<TEntity> query, TGetListInput input)
        {
            //Try to use paging if available
            if (input is IPageDto pageDto)
            {
                return query.Page(pageDto.Page + 1, pageDto.Count);
            }

            //No paging
            return query;
        }

        /// <summary>
        /// Should apply sorting if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual ISelect<TEntity> ApplySorting(ISelect<TEntity> query, TGetListInput input)
        {
            //Try to sort query if available
            if (input is ISortedResultRequest sortInput)
            {
                if (!sortInput.Sorting.IsNullOrWhiteSpace())
                {
                    return query.OrderBy(sortInput.Sorting);
                }
            }

            //IQueryable.Task requires sorting, so we should sort if Take will be used.
            if (input is ILimitedResultRequest)
            {
                return query.OrderByDescending(e => e.Id);
            }

            //No sorting
            return query;
        }

        public async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            var select = CreateFilteredQuery(input);
            long totalCount = await select.CountAsync();
            ApplySorting(select, input);
            List<TEntity> entities = await ApplyPaging(select, input).ToListAsync();
            return new PagedResultDto<TGetListOutputDto>(entities.Select(MapToGetListOutputDto).ToList(), totalCount);
        }

        public async Task<TGetOutputDto> GetAsync(TKey id)
        {
            TEntity entity = await Repository.GetAsync(id);
            return Mapper.Map<TGetOutputDto>(entity);
        }

        public async Task<TGetOutputDto> CreateAsync(TCreateInput createInput)
        {
            TEntity entity = Mapper.Map<TEntity>(createInput);
            await Repository.InsertAsync(entity);
            return Mapper.Map<TGetOutputDto>(entity);
        }

        public async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput updateInput)
        {
            TEntity entity = await GetEntityByIdAsync(id);
            Mapper.Map(updateInput, entity);
            await Repository.UpdateAsync(entity);
            return Mapper.Map<TGetOutputDto>(entity);
        }

        public async Task DeleteAsync(TKey id)
        {
            await Repository.DeleteAsync(id);
        }

        protected virtual async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            return await Repository.GetAsync(id);
        }
    }
}