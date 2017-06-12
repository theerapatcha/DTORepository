using DTORepository.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Internal
{
    internal static class EfExtension
    {

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> KeyCache = new ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>>();

        /// <summary>
        /// This returns PropertyInfos for all the properties in the class that are found in the entity framework metadata 
        /// </summary>
        /// <typeparam name="TClass">The class must belong to a class that entity framework has in its metadata</typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<PropertyInfo> GetKeyProperties<TEntity>(this DbContext context) where TEntity : class
        {
            return KeyCache.GetOrAdd(typeof(TEntity), type => FindKeys(type, context));
        }
        public static object[] GetKeyValues<TEntity>(this DbContext context, object dto)
            where TEntity : class, new()
        {
            var efkeyProperties = context.GetKeyProperties<TEntity>().ToArray();
            var dtoProperties = dto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var keysInOrder = efkeyProperties.Select(x => dtoProperties.SingleOrDefault(y => y.Name == x.Name && y.PropertyType == x.PropertyType)).ToArray();

            if (keysInOrder.Any(x => x == null))
                throw new MissingPrimaryKeyException("The dto must contain all the key(s) properties from the data class.");

            return keysInOrder.Select(x => x.GetValue(dto)).ToArray();
        }
        public static IReadOnlyCollection<PropertyInfo> GetKeyProperties(this DbContext context, Type entityType)
        {
            return KeyCache.GetOrAdd(entityType, type => FindKeys(type, context));
        }
        private static List<PropertyInfo> FindKeys(Type type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .SingleOrDefault(e => objectItemCollection.GetClrType(e) == type);

            if (entityType == null)
                throw new InvalidOperationException("This method expects a entity class. Did you provide a DTO by mistake?");

            var keyProperties = entityType.KeyProperties.Select(x => type.GetProperty(x.Name)).ToList();
            if (!keyProperties.Any())
                throw new MissingPrimaryKeyException(string.Format("Failed to find a EF primary key in type {0}", type.Name));
            if (keyProperties.Any(x => x == null))
                throw new NullReferenceException(string.Format("Failed to find key property by name in type {0}", type.Name));

            return keyProperties;
        }

        public static bool Exists(this DbContext dbContext, Type type)
        {
            string entityName = type.Name;
            ObjectContext objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            MetadataWorkspace workspace = objContext.MetadataWorkspace;
            return workspace.GetItems<EntityType>(DataSpace.CSpace).Any(e => e.Name == entityName);
        }
        public static TEntity FindUntracked<TEntity>(this DbContext dbContext, object[] identifiers)
            where TEntity : class
        {
            return dbContext.Set<TEntity>()
                  .Where(BuildFilter.CreateFilter<TEntity>(dbContext.GetKeyProperties<TEntity>(), identifiers))
                  .AsNoTracking()
                  .SingleOrDefault();
        }
        public static void DetachAll(this DbContext dbContext)
        {
            foreach (DbEntityEntry entityEntry in dbContext.ChangeTracker.Entries().ToArray())
            {
                if (entityEntry.Entity != null)
                {
                    entityEntry.State = EntityState.Detached;
                }
            }
        }
    }
}
