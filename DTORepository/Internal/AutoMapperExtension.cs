using AutoMapper;
using DTORepository.Attributes;
using DTORepository.Common;
using DTORepository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Internal
{
    internal static class AutoMapperExtension
    {
        public static T GetItem<T>(this IMappingOperationOptions options, string key, T defaultValue)
        {
            object obj;
            if (options.Items.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            return defaultValue;
        }
        public static IEnumerable<Type> GetAllMappableTypes(this AppDomain appDomain)
        {
            var allDtoTypes = appDomain.GetAssemblies().SelectMany(x =>
            {
                try
                {
                    return x.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(type => type != null);
                }
            }).Where(t =>
            t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Mappable)));
            return allDtoTypes;
        }
        public static IMappingExpression<TDto, TEntity> ConstructUsingExistingObject<TDto, TEntity>(this IMappingExpression<TDto, TEntity> expression)
            where TEntity : class, new()
            where TDto : DtoBase<TEntity, TDto>, new()
        {
            expression.ConstructUsing((src, opts) =>
            {
                var dbContext = (DbContext)opts.Options.Items["DbContext"];
                var srcEntity = src.FindItemTrackedForUpdate(dbContext);
                if (srcEntity == null) return dbContext.Set<TEntity>().Create();
                return srcEntity;
            });
            return expression;
        }
        public static IMappingExpression<TDto, TEntity> AfterMapSetEntityState<TDto, TEntity>(this IMappingExpression<TDto, TEntity> expression)
            where TEntity: class, new()
            where TDto: DtoBase<TEntity, TDto>, new()
        {
            expression.AfterMap((src, dest, opts) =>
             {
                 var status = (ISuccessOrErrors)opts.Options.Items["CurrentStatus"];
                 var dbContext = (DbContext)opts.Options.Items["DbContext"];
                 var srcEntity = src.FindItemUntracked(dbContext);
                 if (srcEntity == null)
                 {
                     status.Combine(src.CreateDataFromDto(dbContext, dest));
                     src.SetupRestOfEntity(dbContext, new TEntity());
                 }
                 else
                 {
                     status.Combine(src.UpdateDataFromDto(dbContext, dest, srcEntity));
                     src.SetupRestOfEntity(dbContext, dest);
                 }
             });
            return expression;
        }

        public static IMappingExpression<TSource, TDest> IgnoreMappingOnNullValue<TSource, TDest>(this IMappingExpression<TSource,TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Condition((source, destination, sourceMember, destMember, cond) =>
                    sourceMember != null
                ));
            return expression;
        }
       

        public static IMapperConfigurationExpression IgnoreMappingOnNullOrIgnoredValue(this IMapperConfigurationExpression expression)
        {
            expression.ForAllPropertyMaps(map =>
            {
                Type sourceType = map.TypeMap.SourceType;
                Type destType = map.TypeMap.DestinationType;
                return sourceType.IsSubclassOf(typeof(DtoBase)) && !destType.IsSubclassOf(typeof(DtoBase));
            },
                    (map, configuration) =>
                    {
                        var prop = map.SourceMember as PropertyInfo;
                        if (prop != null)
                        {
                            configuration.Condition((src, dest, srcMember, destMember) =>
                            {
                                IgnoreMappingIfValueAttribute attribute = map.SourceMember.GetCustomAttributes().OfType<IgnoreMappingIfValueAttribute>().SingleOrDefault();
                                if (attribute != null)
                                {
                                    return !Object.Equals(srcMember, attribute.IgnoredValue);
                                }
                                else
                                {
                                    return prop.GetValue(src) != null && srcMember != null;
                                }
                            });
                        }
                    }
                );
            return expression;
        }
        public static IMapperConfigurationExpression IgnoreWritingAttributeForActionProperty(this IMapperConfigurationExpression expression)
        {
            expression.ForAllPropertyMaps(map => {
                Type sourceType = map.TypeMap.SourceType;
                Type destType = map.TypeMap.DestinationType;
                if (sourceType.IsSubclassOf(typeof(DtoBase)) && !destType.IsSubclassOf(typeof(DtoBase)) && map.SourceMember != null)
                    return map.SourceMember.GetCustomAttributes().OfType<IgnoreWritingForAttribute>().Any();
                return false;
            },
                    (map, configuration) =>
                    {
                        Type sourceType = map.TypeMap.SourceType;
                        Type destType = map.TypeMap.DestinationType;

                        configuration.PreCondition((opts) =>
                        {
                            ActionFlags actions = opts.Options.GetItem("ActionFlags", ActionFlags.Create | ActionFlags.Update);
                            IgnoreWritingForAttribute attribute = map.SourceMember.GetCustomAttributes().OfType<IgnoreWritingForAttribute>().SingleOrDefault();
                            if (attribute != null)
                            {
                                if (attribute.actions.HasFlag(actions))
                                {
                                    return false;
                                }
                            }
                            return true;
                        });

                    }
                );
            return expression;
        }
        public static IMapperConfigurationExpression IgnoreRetrievingAttributeForActionProperty(this IMapperConfigurationExpression expression)
        {
            expression.ForAllPropertyMaps(map => {
                Type sourceType = map.TypeMap.SourceType;
                Type destType = map.TypeMap.DestinationType;
                if (!sourceType.IsSubclassOf(typeof(DtoBase)) && destType.IsSubclassOf(typeof(DtoBase)) && map.DestinationProperty != null)
                    return map.DestinationProperty.GetCustomAttributes().OfType<IgnoreRetrievingForAttribute>().Any();
                return false;
            },
                    (map, configuration) =>
                    {
                        Type sourceType = map.TypeMap.SourceType;
                        Type destType = map.TypeMap.DestinationType;

                        configuration.PreCondition((opts) =>
                        {
                            ActionFlags actions = opts.Options.GetItem("ActionFlags", ActionFlags.Get | ActionFlags.List);
                            IgnoreRetrievingForAttribute attribute = map.DestinationProperty.GetCustomAttributes().OfType<IgnoreRetrievingForAttribute>().SingleOrDefault();
                            if (attribute != null)
                            {
                                if (attribute.actions.HasFlag(actions))
                                {
                                    return false;
                                }
                            }
                            return true;
                        });
                    }
                );
            return expression;
        }
        //public static IMappingExpression<TSource, TDest> AllowMappingCollection<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        //{
        //   expression.ForAllMembers(opt => opt.Condition((source, destination, sourceMember, destMember, cond) =>
        //            sourceMember != null
        //        ));
        //    return expression;
        //}
    }
}
