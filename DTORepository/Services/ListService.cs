using AutoMapper;
using AutoMapper.QueryableExtensions;
using DTORepository.Common;
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
    public interface IListService<TEntity>
        where TEntity : class, new()
    {
        IQueryable<TDto> List<TDto>()
            where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new();

    }
    public class ListService<TEntity> : IListService<TEntity>
        where TEntity : class, new()
    {
        public DbContext dbContext { get; }
        public ListService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<TDto> List<TDto>()
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.List))
                throw new InvalidOperationException("Dto is not allowed for this kind of action");
            return dbContext.Set<TEntity>().ProjectTo<TDto>(DTORepositoryContainer.Mapper.ConfigurationProvider, new Dictionary<string, object>
            {
                ["DbContext"] = dbContext,
                ["ActionFlags"] = ActionFlags.List
            });
        }
        public ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new()
        {

            var status = new SuccessOrErrors<IList<TDto>>();
            //var dtos = dbContext.Set<TEntity>().Where(predicate).ProjectTo<TDto>(DtoHelper.Mapper.ConfigurationProvider).ToList();
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.List))
                return status.AddSingleError("Dto is not allowed for this kind of action");
            var entities = dbContext.Set<TEntity>().Where(predicate);
            var dtos = DTORepositoryContainer.Mapper.Map<IList<TDto>>(entities, opts => opts.Items["ActionFlags"] = ActionFlags.List);
            return status.SetSuccessWithResult(dtos, "Success");
        
        }
        public Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
           where TDto : DtoBase<TEntity, TDto>, new()
        {
            return Task.Run(() =>
            {
                return this.Query<TDto>(predicate);
            });
        }
    }
}
