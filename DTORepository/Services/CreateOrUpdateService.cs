using AutoMapper;
using DTORepository.Common;
using DTORepository.Internal;
using DTORepository.Models;
using DTORepository.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DTORepository.Services
{
    public interface ICreateOrUpdateService<TEntity>
        where TEntity : class, new()
    {
        ISuccessOrErrors<TDto> CreateOrUpdate<TDto>(TDto obj, ActionFlags actions = ActionFlags.Create | ActionFlags.Update)
            where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> CreateOrUpdateAsync<TDto>(TDto obj, ActionFlags actions = ActionFlags.Create | ActionFlags.Update)
            where TDto : DtoBase<TEntity, TDto>, new();
    }
    public class CreateOrUpdateService<TEntity> : ICreateOrUpdateService<TEntity>
        where TEntity : class, new()
    {
        public DbContext dbContext { get; }
        public CreateOrUpdateService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ISuccessOrErrors<TDto> CreateOrUpdate<TDto>(TDto dto, ActionFlags actions) 
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                var status = SuccessOrErrors<TDto>.SuccessWithResult(null, "Success");
                status = validateDtoState(status, dto, actions);
                if (status.IsValid)
                {
                    var entity = DTORepositoryContainer.Mapper.Map<TEntity>(dto, opts =>
                    {
                        opts.Items["DbContext"] = dbContext;
                        opts.Items["CurrentStatus"] = status;
                        opts.Items["ActionFlags"] = actions;
                    });
                    if (status.IsValid)
                    {
                        dbContext.SaveChanges();
                        DTORepositoryContainer.Mapper.Map<TEntity, TDto>(entity, dto);
                        return status.SetSuccessWithResult(dto, "Success");
                    }
                }
                return status;
            } catch(Exception e)
            {
                var throwOnError = false;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }

        public async Task<ISuccessOrErrors<TDto>> CreateOrUpdateAsync<TDto>(TDto dto, ActionFlags actions)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                var status = SuccessOrErrors<TDto>.SuccessWithResult(null, "Success");
                status = validateDtoState(status, dto, actions);
                if (status.IsValid)
                {
                    var entity = DTORepositoryContainer.Mapper.Map<TEntity>(dto, opts =>
                    {
                        opts.Items["DbContext"] = dbContext;
                        opts.Items["CurrentStatus"] = status;
                        opts.Items["ActionFlags"] = actions;

                    });
                    if (status.IsValid)
                    {
                        await dbContext.SaveChangesAsync();
                        DTORepositoryContainer.Mapper.Map<TEntity, TDto>(entity, dto);
                        return status.SetSuccessWithResult(dto, "Success");
                    }
                }
                return status;
            }
            catch (Exception e)
            {
                var throwOnError = false;
                if (throwOnError)
                {
                    throw e;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        private ISuccessOrErrors<TDto> validateDtoState<TDto>(ISuccessOrErrors<TDto> status, TDto dto, ActionFlags flags)
            where TDto : DtoBase<TEntity, TDto>, new()
        {            
            var currentItem = dto.FindItemTrackedForUpdate(dbContext);
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.Update) && currentItem != null)
                return status.AddSingleError("Dto is not allowed for this kind of action");
            if (!new TDto().AllowedActions.HasFlag(ActionFlags.Create) && currentItem == null)
                return status.AddSingleError("Dto is not allowed for this kind of action");
            if (!flags.HasFlag(ActionFlags.Update) && currentItem != null)
            {
                return status.AddSingleError("Object already exists");
            }
            if (!flags.HasFlag(ActionFlags.Create) && currentItem == null)
            {
                return status.AddSingleError("Object doesn't exist");
            }
            
            return status;
        }
    }
}
