using DTORepository.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DTORepository.Common
{
    public class DTOQueryable<TEntity, TDto> : IQueryable<TDto>, IOrderedQueryable<TDto>, IQueryProvider, IDbAsyncEnumerable<TDto>
        where TDto : DtoBase
        where TEntity: class,new()
    {
        DbContext dbContext;
        IQueryable<TEntity> queryable;
        Action<AutoMapper.IMappingOperationOptions> opts;
        IQueryable<TDto> dtoQueryable;
        List<Expression<Func<TDto, bool>>> _tmpDtoExpression = new List<Expression<Func<TDto, bool>>>();
        public DTOQueryable(DbContext dbContext)
        {

            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
            this.dtoQueryable = new List<TDto>().AsQueryable();
        }
        public DTOQueryable(DbContext dbContext, Action<AutoMapper.IMappingOperationOptions> opts)
        {
            
            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
            this.dtoQueryable = new List<TDto>().AsQueryable();
            this.opts = opts;
        }
        DTOQueryable(DbContext dbContext, Action<AutoMapper.IMappingOperationOptions> opts, IQueryable<TEntity> queryable)
        {
            this.dbContext = dbContext;
            this.queryable = queryable;
            this.dtoQueryable = new List<TDto>().AsQueryable();
            this.opts = opts;
        }
        Type IQueryable.ElementType
        {
            get
            {
                return typeof(TDto);
            }
        }

        Expression IQueryable.Expression
        {
            get
            {
                return Expression.Constant(this);
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this;
            }
        }

        public IEnumerator<TDto> GetEnumerator()
        {
            var list = DTORepositoryContainer.Mapper.Map<List<TDto>>(queryable.ToList(), opts).AsQueryable();
            foreach(var dtoExpr in _tmpDtoExpression)
            {
                list = list.Where(dtoExpr);
            }
            foreach(var item in list.ToList())
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var dtoMethodCallExpression = expression as MethodCallExpression;
            var entityMethodCallExpression = ConvertMethodCallExpression<TDto, TEntity>(dtoMethodCallExpression, this.queryable);
            
            return (IQueryable<TElement>) new DTOQueryable<TEntity, TDto>(dbContext, opts, this.queryable.Provider.CreateQuery<TEntity>(entityMethodCallExpression));
        }
        public object Execute(Expression expression)
        {
            return Execute<TDto>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            try
            {
                var list = new List<TDto>();
                var enumerator = GetEnumerator();
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
                var dtoMethodCallExpression = expression as MethodCallExpression;
                var entityMethodCallExpression = ConvertMethodCallExpression<TDto, TDto>(dtoMethodCallExpression, list.AsQueryable());
                return list.AsQueryable().Provider.Execute<TResult>(entityMethodCallExpression);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private MethodCallExpression ConvertMethodCallExpression<TSrc, TDest>(MethodCallExpression expression, IQueryable targetQueryable)
        {
            var dtoMethodCallExpression = expression as MethodCallExpression;
            MethodInfo entityMethod = dtoMethodCallExpression.Method.ReflectedType.GetMethods()
                .Where(m => m.MetadataToken == dtoMethodCallExpression.Method.MetadataToken)
                .Single()
                .MakeGenericMethod(
                    dtoMethodCallExpression.Method.GetGenericArguments().Select(x => {
                        if (x == typeof(TSrc)) return typeof(TDest);
                        else return x;
                    }).ToArray()
                );
            var dtoArgs = dtoMethodCallExpression.Arguments;
            var entityArgs = entityMethod.GetParameters()
                .Select((x, i) =>
                {
                    if (x.ParameterType == typeof(IQueryable<TDest>)) return targetQueryable.Expression;
                    else if (dtoArgs[i] is UnaryExpression)
                    {
                        var unaryExp = dtoArgs[i] as UnaryExpression;
                        return (Expression)DTORepositoryContainer.Mapper.Map(unaryExp.Operand, unaryExp.Operand.GetType(), x.ParameterType);
                    }
                    return dtoArgs[i];
                }).ToArray();

            return Expression.Call(entityMethod, entityArgs);
        }
        public IDbAsyncEnumerator<TDto> GetAsyncEnumerator()
        {
            return new DbAsyncEnumerator<TDto>(this.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }
    internal class DbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public DbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}
