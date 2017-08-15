using DTORepository.Common;
using DTORepository.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DTORepository.Core
{
    public class UnitOfWork<TContext> : IDisposable
        where TContext: DbContext
    {
        TContext dbContext;
        public UnitOfWork(TContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public T Execute<T>(Func<TContext, T> executeContext)
            where T: ISuccessOrErrors
        {
            using (var scope = new TransactionScope())
            {
                T status = executeContext(this.dbContext);
                if (status.IsValid)
                {
                    scope.Complete();
                } else {
                    this.dbContext.DetachAll();
                }
                return status;
            }
        }
        
        public async Task<T> ExecuteAsync<T>(Func<TContext, Task<T>> executeContext)
            where T: ISuccessOrErrors
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                T status = await executeContext(this.dbContext);
                if (status.IsValid)
                {
                    scope.Complete();
                }else {
                    this.dbContext.DetachAll();
                }
                return status;
            }
        }
      

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
