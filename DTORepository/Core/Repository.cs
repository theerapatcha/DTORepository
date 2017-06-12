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
        ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate)
            where TDto : DtoBase<TEntity, TDto>, new();
        Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
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
            return this.detailService.Get<TDto>(identifiers);
        }
        public Task<ISuccessOrErrors<TDto>> GetAsync<TDto>(params object[] identifiers)
             where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.detailService.GetAsync<TDto>(identifiers);
        }
        public ISuccessOrErrors<IList<TDto>> Query<TDto>(Expression<Func<TEntity, bool>> predicate)
          where TDto : DtoBase<TEntity, TDto>, new()
        {
            return listService.Query<TDto>(predicate);
        }
        public Task<ISuccessOrErrors<IList<TDto>>> QueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
         where TDto : DtoBase<TEntity, TDto>, new()
        {
            return listService.QueryAsync<TDto>(predicate);
        }

        public ISuccessOrErrors<TDto> Create<TDto>(TDto dto)
            where TDto: DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdate(dto, ActionFlags.Create);
        }
        public Task<ISuccessOrErrors<TDto>> CreateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdateAsync(dto, ActionFlags.Create);
        }
        public IQueryable<TDto> List<TDto>()
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return listService.List<TDto>();
        }
      
        public ISuccessOrErrors<TDto> Update<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdate(dto, ActionFlags.Update);
        }
        public Task<ISuccessOrErrors<TDto>> UpdateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdateAsync(dto, ActionFlags.Update);
        }
        public ISuccessOrErrors<TDto> CreateOrUpdate<TDto>(TDto dto)
           where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdate(dto);
        }
        public Task<ISuccessOrErrors<TDto>> CreateOrUpdateAsync<TDto>(TDto dto)
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            return this.createOrUpdateService.CreateOrUpdateAsync(dto);
        }
        public ISuccessOrErrors Delete(params object[] identifiers)
        {
            return this.deleteService.Delete(identifiers);
        }
        public Task<ISuccessOrErrors> DeleteAsync(params object[] identifiers)
        {
            return this.deleteService.DeleteAsync(identifiers);
        }
    }
}
