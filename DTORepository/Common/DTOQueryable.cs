using DTORepository.Internal;
using DTORepository.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Common
{
    public class DTOQueryable<TEntity, TDto> : IQueryable<TDto>, IQueryProvider
        where TDto : DtoBase
        where TEntity: class,new()
    {
        DbContext dbContext;
        IQueryable<TEntity> queryable;
        Action<AutoMapper.IMappingOperationOptions> opts;

        List<Expression<Func<TDto, bool>>> _tmpDtoExpression = new List<Expression<Func<TDto, bool>>>();
        public DTOQueryable(DbContext dbContext)
        {

            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
        }
        public DTOQueryable(DbContext dbContext, Action<AutoMapper.IMappingOperationOptions> opts)
        {
            
            this.dbContext = dbContext;
            this.queryable = dbContext.Set<TEntity>().AsQueryable();
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
            Expression<Func<TDto, bool>> dtoExpr = GetWhereLambda(expression as MethodCallExpression);
            try
            {
                var a = DTORepositoryContainer.Mapper.Map<Expression<Func<TEntity, bool>>>(dtoExpr);
                this.queryable = this.queryable.Where(a);
            }
            catch (Exception e) {
                _tmpDtoExpression.Add(dtoExpr);
            }
            return (IQueryable<TElement>) this;
        }
        private BinaryExpression GenerateEntityExpression(Expression left, ExpressionType type, Expression right)
        {
            return null;
        }
        public object Execute(Expression expression)
        {
            Expression<Func<TDto, bool>> dtoExpr = GetWhereLambda(expression as MethodCallExpression);
            try
            {
                var a = DTORepositoryContainer.Mapper.Map<Expression<Func<TEntity, bool>>>(dtoExpr);
                return this.queryable.Provider.Execute(a);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Expression<Func<TDto, bool>> dtoExpr = GetWhereLambda(expression as MethodCallExpression);
            try
            {
                var a = DTORepositoryContainer.Mapper.Map<Expression<Func<TEntity, bool>>>(dtoExpr);
                return this.queryable.Provider.Execute<TResult>(a);
            }
            catch (Exception e)
            {
                return default(TResult);
            }
        }

        public Expression<Func<TDto, bool>> GetWhereLambda(MethodCallExpression methodCall)
        {
            Func<IQueryable<TEntity>, Expression<Func<TEntity, Boolean>>, IQueryable<TEntity>> whereDelegate = Queryable.Where;
            var whereLambdaQuote = methodCall.Arguments.Where(x=> x is UnaryExpression).Select(x => x as UnaryExpression).FirstOrDefault();
            return whereLambdaQuote?.Operand as Expression<Func<TDto, bool>>;
        }

    }
}
