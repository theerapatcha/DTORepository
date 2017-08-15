using AutoMapper;
using AutoMapper.XpressionMapper.Extensions;
using DTORepository.Internal;
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
    public class DTOQueryable<TContext, TEntity, TDto> : IQueryable<TDto>, IOrderedQueryable<TDto>, IQueryProvider, IDbAsyncEnumerable<TDto>
        where TContext: DbContext
        where TDto : DtoBase<TContext, TEntity, TDto>, new()
        where TEntity: class,new()
    {
        TContext dbContext;
        IQueryable<TEntity> queryable;
        Action<AutoMapper.IMappingOperationOptions> opts;
        IQueryable<TDto> dtoQueryable;
        List<Expression<Func<TDto, bool>>> _tmpDtoExpression = new List<Expression<Func<TDto, bool>>>();
        public DTOQueryable(TContext dbContext)
        {

            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
            this.dtoQueryable = new List<TDto>().AsQueryable();
        }
        public DTOQueryable(TContext dbContext, Action<AutoMapper.IMappingOperationOptions> opts)
        {
            
            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
            this.dtoQueryable = new List<TDto>().AsQueryable();
            this.opts = opts;
        }
        public DTOQueryable(TContext dbContext, Action<AutoMapper.IMappingOperationOptions> opts, IQueryable<TEntity> queryable)
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

        public IQueryable<TDtoElement> CreateQuery<TDtoElement>(Expression expression)
        {
            if (typeof(TDto) != typeof(TDtoElement)) throw new InvalidOperationException("Unsupport operation. Yet to be implemented");

            var dtoMethodCallExpression = expression as MethodCallExpression;
            var entityMethodCallExpression = ConvertMethodCallExpression<TDto, TEntity>(dtoMethodCallExpression, this.queryable);
            
            Type entityElementType = typeof(TDtoElement).ToEntityType();
            Type myParameterizedSomeClass = typeof(DTOQueryable<,,>).MakeGenericType(typeof(TContext), entityElementType, typeof(TDtoElement));
            ConstructorInfo constr = myParameterizedSomeClass.GetConstructor(new[] { typeof(TContext), typeof(Action<AutoMapper.IMappingOperationOptions>), typeof(IQueryable<>).MakeGenericType(entityElementType) });
            return (IQueryable<TDtoElement>) constr.Invoke(new object[] { dbContext, opts, this.queryable.Provider.CreateQuery(entityMethodCallExpression) });
        }
        public object Execute(Expression expression)
        {
            return Execute<TDto>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
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
                    if (x.ParameterType == typeof(IQueryable<TDest>) || 
                    x.ParameterType.GetInterfaces().Any(type => type == typeof(IQueryable<TDest>)))
                        return targetQueryable.Expression;
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
