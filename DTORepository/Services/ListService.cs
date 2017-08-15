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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Services
{
    public interface IListService<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        //IQueryable<TDto> List<TDto>()
        //    where TDto : DtoBase<TEntity, TDto>, new();
        IQueryable<TDto> List<TDto>(Expression<Func<TEntity, bool>> predicate = null)
            where TDto : DtoBase<TContext, TEntity, TDto>, new(); 
        ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate = null)
            where TDto : DtoBase<TContext, TEntity, TDto>, new();
        Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate = null)
            where TDto : DtoBase<TContext, TEntity, TDto>, new();

    }
    public class ListService<TContext, TEntity> : IListService<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        public TContext dbContext { get; }
        public ListService(TContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //public IQueryable<TDto> List<TDto>()
        //    where TDto : DtoBase<TEntity, TDto>, new()
        //{
        //    return List<TDto>(null);
        //}
        public IQueryable<TDto> List<TDto>(Expression<Func<TEntity, bool>> predicate)
            where TDto : DtoBase<TContext, TEntity, TDto>, new()
        {
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.List))
                throw new InvalidOperationException("Dto is not allowed for this kind of action");
            var dbQueryable = dbContext.Set<TEntity>().AsQueryable();
            if (predicate != null) dbQueryable = dbQueryable.Where(predicate);
            return new DTOQueryable<TContext, TEntity, TDto>(dbContext, opts => {
                opts.Items["ActionFlags"] = ActionFlags.List;
                opts.Items["DbContext"] = dbContext;
            }, dbQueryable);
        }
        public ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate = null)
            where TDto : DtoBase<TContext, TEntity, TDto>, new()
        {

            var status = new SuccessOrErrors<IList<TDto>>();
            //var dtos = dbContext.Set<TEntity>().Where(predicate).ProjectTo<TDto>(DtoHelper.Mapper.ConfigurationProvider).ToList();
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.List))
                return status.AddSingleError("Dto is not allowed for this kind of action");
            var dtos = new DTOQueryable<TContext, TEntity, TDto>(dbContext, opts =>
            {
                opts.Items["ActionFlags"] = ActionFlags.List;
                opts.Items["DbContext"] = dbContext;
            }, dbContext.Set<TEntity>().Where(predicate)).ToList();
            
            return status.SetSuccessWithResult(dtos, "Success");
        
        }
        public Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate = null)
           where TDto : DtoBase<TContext, TEntity, TDto>, new()
        {
            return Task.Run(() =>
            {
                return this.Query<TDto>(predicate);
            });
        }
    }
}
