using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Core
{
    public class UnitOfWorkFactory
    {
        DbContext dbContext;
        public UnitOfWorkFactory(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(dbContext);
        }
    }
}