﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Internal
{
    internal static class BuildFilter
    {

        public static Expression<Func<T, bool>> CreateFilter<T>(IReadOnlyCollection<PropertyInfo> keyProperties, object[] keyValues)
        {
            if (keyProperties.Count != keyValues.Length)
                throw new ArgumentException("The number of keys values provided does not match the number of keys in the entity class.");

            var x = Expression.Parameter(typeof(T), "x");
            var filterParts = keyProperties.Select((t, i) => BuildEqual<T>(x, t, keyValues[i])).ToList();
            var combinedFilter = CombineFilters(filterParts);

            return Expression.Lambda<Func<T, bool>>(combinedFilter, x);
        }

        private static Expression CombineFilters(List<BinaryExpression> filterParts)
        {
            var result = filterParts.First();
            for (int i = 1; i < filterParts.Count; i++)
                result = Expression.AndAlso(result, filterParts[i]);

            return result;
        }

        private static BinaryExpression BuildEqual<T>(ParameterExpression p, PropertyInfo prop, object expectedValue)
        {
            var m = Expression.Property(p, prop);
            var c = Expression.Constant(expectedValue);
            var ex = Expression.Equal(m, c);
            return ex;
        }

    }
}
