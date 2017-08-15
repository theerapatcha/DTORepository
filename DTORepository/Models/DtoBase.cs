using AutoMapper;
using DTORepository.Attributes;
using DTORepository.Common;
using DTORepository.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Models
{
    public abstract class DtoBase : Mappable {
        protected internal virtual ActionFlags AllowedActions => ActionFlags.All;
    }
    
    public abstract class DtoBase<TContext, TEntity, TDto> : DtoBase
        where TContext: DbContext
        where TEntity: class, new()
        where TDto: DtoBase<TContext, TEntity, TDto>, new()
    {   
        public override Profile GetMapperProfile()
        {
            var profile = new MappableProfile();
            profile.CreateMap<TEntity, TEntity>();
            var entityToDtoMapping = CreateEntityToDtoMapping(profile);
            var insertMapping =  CreateDtoToEntityMapping(profile);
            return profile;
        }
        private IMappingExpression<TEntity, TDto> CreateEntityToDtoMapping(Profile profile)
        {
            var expression = profile.CreateMap<TEntity, TDto>();
            if(EntityToDtoProjection != null)
            {
                EntityToDtoProjection.Invoke(expression);
            }
            expression.AfterMap((entity, dto, ctx) =>
            {
                var a = ctx.Items.ContainsKey("DbContext");
                if(!a)
                {
                    var x = a;
                }
                var context = (TContext) ctx.Items["DbContext"];
                dto.SetupRestOfDto(context, entity);
            });
            
            
            return expression;
        }
        private IMappingExpression<TDto, TEntity> CreateDtoToEntityMapping(Profile profile)
        {
            return profile.CreateMap<TDto, TEntity>()
                .ConstructUsingExistingObject<TContext, TDto, TEntity>()
                .AfterMapSetEntityState<TContext, TDto, TEntity>();
        }
        protected internal virtual TDto SetupRestOfDto(TContext context, TEntity entity)
        {
            return (TDto) this;
        }
        protected internal virtual TEntity SetupRestOfEntity(TContext context, TEntity entity)
        {
            return entity;
        }
        protected internal virtual Action<IMappingExpression<TEntity, TDto>> EntityToDtoProjection => null;
        protected internal virtual ISuccessOrErrors<TEntity> CreateDataFromDto(TContext context, TEntity destination)
        {
            context.Set<TEntity>().Add(destination);
            return SuccessOrErrors<TEntity>.SuccessWithResult(destination, "Success");
        }
        protected internal virtual ISuccessOrErrors<TEntity> UpdateDataFromDto(TContext context, TEntity destination, TEntity original)
        {
            context.Entry(original).State = EntityState.Detached;
            context.Entry(destination).State = EntityState.Modified;
            return SuccessOrErrors<TEntity>.SuccessWithResult(destination, "Success");
        }
        protected internal virtual TEntity FindItemTrackedForUpdate(TContext context)
        {
            var keyValues = context.GetKeyValues<TEntity>(this);
            if (!keyValues.Intersect(new List<object> { null, 0 }).Any())
                return context.Set<TEntity>().Find(keyValues);
            return null;
        }
        protected internal virtual TEntity FindItemUntracked(TContext context)
        {
            var keyValues = context.GetKeyValues<TEntity>(this);
            if (!keyValues.Intersect(new List<object> { null, 0 }).Any())
                return context.FindUntracked<TEntity>(keyValues);
            return null;
        }

    }
}
