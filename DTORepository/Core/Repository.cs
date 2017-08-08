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

namespace DTORepository.Core
{
    public interface IRepository<TEntity>
        where TEntity: class, new()
    {
        IQueryable<TDto> List<TDto>() where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TDto, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TDto, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers) where TDto : DtoBase<TEntity,TDto>, new();
        Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers) where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<TDto> Create<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> CreateAsync<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<TDto> Update<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> UpdateAsync<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors<TDto> CreateOrUpdate<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<TDto>> CreateOrUpdateAsync<TDto>(TDto dto) where TDto : DtoBase<TEntity, TDto>, new();
        ISuccessOrErrors Delete(params object[] identifiers);
        Task<ISuccessOrErrors> DeleteAsync(params object[] identifiers);
    }
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, new()
    {
        private ICreateOrUpdateService<TEntity> createOrUpdateService;
        private IDetailService<TEntity> detailService;
        private IListService<TEntity> listService;
        private IDeleteService<TEntity> deleteService;
        public Repository(ICreateOrUpdateService<TEntity> createOrUpdateService,
            IDetailService<TEntity> detailService,
            IListService<TEntity> listService,
            IDeleteService<TEntity> deleteService)
        {
            this.createOrUpdateService = createOrUpdateService;
            this.detailService = detailService;
            this.listService = listService;
            this.deleteService = deleteService;

        }

        public ISuccessOrErrors<TDto> Get<TDto>(params object[] identifiers)
             where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return this.detailService.Get<TDto>(identifiers);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
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
            try
            { 
                return await this.detailService.GetAsync<TDto>(identifiers);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TDto, bool>> predicate)
          where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return listService.Query<TDto>(predicate);
            }
             catch(Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<IList<TDto>>.ConvertNonResultStatus(err);
            }
}
        public async Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TDto, bool>> predicate)
         where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return await listService.QueryAsync<TDto>(predicate);
            }
            catch(Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<IList<TDto>>.ConvertNonResultStatus(err);
            }
}

        public ISuccessOrErrors<TDto> Create<TDto>(TDto dto)
            where TDto: DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return this.createOrUpdateService.CreateOrUpdate(dto, ActionFlags.Create);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public async Task<ISuccessOrErrors<TDto>> CreateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return await this.createOrUpdateService.CreateOrUpdateAsync(dto, ActionFlags.Create);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public IQueryable<TDto> List<TDto>()
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return listService.List<TDto>();
        }
      
        public ISuccessOrErrors<TDto> Update<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return this.createOrUpdateService.CreateOrUpdate(dto, ActionFlags.Update);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public async Task<ISuccessOrErrors<TDto>> UpdateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return await this.createOrUpdateService.CreateOrUpdateAsync(dto, ActionFlags.Update);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
}
        public ISuccessOrErrors<TDto> CreateOrUpdate<TDto>(TDto dto)
           where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return this.createOrUpdateService.CreateOrUpdate(dto);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public async Task<ISuccessOrErrors<TDto>> CreateOrUpdateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            try
            {
                return await this.createOrUpdateService.CreateOrUpdateAsync(dto);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return SuccessOrErrors<TDto>.ConvertNonResultStatus(err);
            }
        }
        public ISuccessOrErrors Delete(params object[] identifiers)
        {
            try
            {
                return this.deleteService.Delete(identifiers);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return err;
            }
        }
        public async Task<ISuccessOrErrors> DeleteAsync(params object[] identifiers)
        {
            try
            {
                return await this.deleteService.DeleteAsync(identifiers);
            }
            catch (Exception e)
            {
                var throwOnError = DTORepositoryContainer.ThrowsOnDatabaseError;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                return err;
            }
        }
    }
}
