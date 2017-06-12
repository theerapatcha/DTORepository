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
    public interface IDeleteService<TEntity>
        where TEntity : class, new()
    {
        ISuccessOrErrors Delete(params object[] identifiers);
        Task<ISuccessOrErrors> DeleteAsync(params object[] identifiers);

    }
    public class DeleteService<TEntity> : IDeleteService<TEntity>
        where TEntity : class, new()
    {
        public DbContext dbContext { get; }
        public DeleteService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ISuccessOrErrors Delete(params object[] identifiers)
        {
            var status = SuccessOrErrors.Success("Delete success");
            try
            {
                var dbSet = this.dbContext.Set<TEntity>();
                var entity = dbSet.Find(identifiers);
                dbSet.Remove(entity);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                var throwOnError = false;
                if (throwOnError)
                {
                    throw;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                status.AddSingleError("Cannot delete", err);
            }
            return status;
        }
        public async Task<ISuccessOrErrors> DeleteAsync(params object[] identifiers)
        {
            var status = SuccessOrErrors.Success("Delete success");
            try
            {
                var dbSet = this.dbContext.Set<TEntity>();
                var entity = await dbSet.FindAsync(identifiers);
                dbSet.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var throwOnError = false;
                if (throwOnError)
                {
                    throw e;
                }
                var err = EntityFrameworkExceptionHandler.HandleException(e);
                status.AddSingleError("Cannot delete", err);
            }
            return status;
        }
    }
}
