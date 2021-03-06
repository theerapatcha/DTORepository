﻿
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DTORepository.Common;
using DTORepository.Internal;
using DTORepository.Models;
using DTORepository.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Services
{
    public interface IDetailService<TContext, TEntity>
        where TContext: DbContext
        where TEntity : class, new()
    {
        ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers)
            where TDto : DtoBase<TContext, TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers)
            where TDto : DtoBase<TContext, TEntity, TDto>, new();
    }
    public class DetailService<TContext, TEntity> : IDetailService<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        public TContext dbContext { get; }
        public DetailService(TContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers)
            where TDto : DtoBase<TContext, TEntity, TDto>, new()
        {
            var status = new SuccessOrErrors<TDto>();

            if (!new TDto().AllowedActions.HasFlag(ActionFlags.Get))
                return status.AddSingleError("Dto is not allowed for this kind of action");
            var entity = dbContext.Set<TEntity>()
              .Where(BuildFilter.CreateFilter<TEntity>(dbContext.GetKeyProperties<TEntity>(), identifiers))
              .SingleOrDefault();
            var dto = DTORepositoryContainer.Mapper.Map<TDto>(entity, opts => {
                opts.Items["ActionFlags"] = ActionFlags.Get;
                opts.Items["DbContext"] = dbContext;
            });
            if (dto != null)
            {
                return status.SetSuccessWithResult(dto, "Success");
            }
            return status.AddSingleError("Not found");
        
        }

        public async Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers)
           where TDto : DtoBase<TContext, TEntity, TDto>, new()
        {
            var status = new SuccessOrErrors<TDto>();

            if (!new TDto().AllowedActions.HasFlag(ActionFlags.Get))
                return status.AddSingleError("Dto is not allowed for this kind of action");
            var entity = await dbContext.Set<TEntity>()
                .Where(BuildFilter.CreateFilter<TEntity>(dbContext.GetKeyProperties<TEntity>(), identifiers))
                .SingleOrDefaultAsync();
            var dto = DTORepositoryContainer.Mapper.Map<TDto>(entity, opts => {
                opts.Items["ActionFlags"] = ActionFlags.Get;
                opts.Items["DbContext"] = dbContext;
            });

            if (dto != null)
            {
                return status.SetSuccessWithResult(dto, "Success");
            }
            return status.AddSingleError("Not found");
        
        }
    }
}
