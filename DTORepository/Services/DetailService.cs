
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
    public interface IDetailService<TEntity>
        where TEntity : class, new()
    {
        ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers)
            where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers)
            where TDto : DtoBase<TEntity, TDto>, new();
    }
    public class DetailService<TEntity> : IDetailService<TEntity>
        where TEntity : class, new()
    {
        public DbContext dbContext { get; }
        public DetailService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            var status = new SuccessOrErrors<TDto>();
            try
            {
                if(!new TDto().AllowedActions.HasFlag(ActionFlags.Get))
                    return status.AddSingleError("Dto is not allowed for this kind of action");
                var entity = dbContext.Set<TEntity>()
                  .Where(BuildFilter.CreateFilter<TEntity>(dbContext.GetKeyProperties<TEntity>(), identifiers))
                  .SingleOrDefault();
                var dto = DTORepositoryContainer.Mapper.Map<TDto>(entity, opts => opts.Items["ActionFlags"] = ActionFlags.Get);
                if (dto != null)
                {
                    return status.SetSuccessWithResult(dto, "Success");
                }
                return status.AddSingleError("Not found");
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }

        public async Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers)
           where TDto : DtoBase<TEntity, TDto>, new()
        {
            var status = new SuccessOrErrors<TDto>();
            try
            {
                if (!new TDto().AllowedActions.HasFlag(ActionFlags.Get))
                    return status.AddSingleError("Dto is not allowed for this kind of action");
                var entity = await dbContext.Set<TEntity>()
                    .Where(BuildFilter.CreateFilter<TEntity>(dbContext.GetKeyProperties<TEntity>(), identifiers))
                    .SingleOrDefaultAsync();
                var dto = DTORepositoryContainer.Mapper.Map<TDto>(entity, opts => opts.Items["ActionFlags"] = ActionFlags.Get);

                if (dto != null)
                {
                    return status.SetSuccessWithResult(dto, "Success");
                }
                return status.AddSingleError("Not found");
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnError;
                if (throwOnError)
                {
                    throw e;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
    }
}
