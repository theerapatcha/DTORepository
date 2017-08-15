using DTORepository.Models;
using DTORepository.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Core
{
    public class RepositoryFactory<TContext>
        where TContext: DbContext
    {
        private TContext dbContext;
        public RepositoryFactory(TContext dbContext)
        {
            this.dbContext = dbContext;
            
        }
        public IRepository<TContext, TEntity> CreateRepository<TEntity>()
            where TEntity: class, new()
        {
            return new Repository<TContext,TEntity>(
                new CreateOrUpdateService<TContext, TEntity>(dbContext),
                new DetailService<TContext, TEntity>(dbContext),
                new ListService<TContext, TEntity>(dbContext),
                new DeleteService<TContext, TEntity>(dbContext)
            );
        }
    }
}
