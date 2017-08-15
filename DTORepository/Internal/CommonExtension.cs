using AutoMapper;
using DTORepository.Attributes;
using DTORepository.Internal;
using DTORepository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Internal
{
    internal static class CommonExtension
    {
        // all error checking left out for brevity

        // a.k.a., linked list style enumerator
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        
        public static bool IsEnumerableType(this Type type)
        {
            return (type.GetInterface("IEnumerable") != null);
        }
        public static bool IsCollectionType(this Type type)
        {
            return (type.GetInterface("ICollection") != null);
        }
        public static Type ToEntityType(this Type type)
        {

            if (type.IsEnumerableType() || type.IsCollectionType())
            {
                return typeof(ICollection<>).MakeGenericType(ToEntityType(type.GenericTypeArguments[0]));
            }
            if (type.IsSubclassOf(typeof(DtoBase)))
            {
                return type.BaseType.GenericTypeArguments[1];
            }
            return type;
        }
    }
}
