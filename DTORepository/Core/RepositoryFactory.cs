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
    public class RepositoryFactory
    {
        private DbContext dbContext;
        public RepositoryFactory(DbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }
        public IRepository<TEntity> CreateRepository<TEntity>()
            where TEntity: class, new()
        {
            return new Repository<TEntity>(
                new CreateOrUpdateService<TEntity>(dbContext),
                new DetailService<TEntity>(dbContext),
                new ListService<TEntity>(dbContext),
                new DeleteService<TEntity>(dbContext)
            );
        }
    }
}
