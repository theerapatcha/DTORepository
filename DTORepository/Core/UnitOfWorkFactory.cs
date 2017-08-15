using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Core
{
    public class UnitOfWorkFactory<TContext>
        where TContext : DbContext
    {
        TContext dbContext;
        public UnitOfWorkFactory(TContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public UnitOfWork<TContext> CreateUnitOfWork()
        {
            return new UnitOfWork<TContext>(dbContext);
        }
    }
}