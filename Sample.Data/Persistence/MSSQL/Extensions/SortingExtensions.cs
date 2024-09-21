using Sample.Common.DTOs.Requests;
using Sample.Common.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace Sample.Data.Persistence.MSSQL.Extensions
{
    public static class SortingExtensions
    {
        public static (IQueryable<T>, bool) Sort<T>(this IQueryable<T> query, ISortable filter, IDictionary<string, Expression<Func<T, object>>> sorting)
        {
            if (string.IsNullOrWhiteSpace(filter.SortingProperty))
                return (query, false);

            try
            {
                if (!sorting.TryGetValue(filter.SortingProperty, out var expression))
                    throw new ArgumentException($"No sort function found for the property '{filter.SortingProperty}'.");

                if (IsPropertyMapped(expression))
                {
                    var type = filter.SortingType is SortingType.Descending ? "OrderByDescending" : "OrderBy";
                    var callExpression = Expression.Call(
                        typeof(Queryable),
                        type,
                        [typeof(T), expression.ReturnType],
                        query.Expression,
                        expression
                    );

                    return (query.Provider.CreateQuery<T>(callExpression), false);
                }
                return (OrderInMemory(query, expression, filter.SortingType), true);
            }
            catch (ArgumentException)
            {
                throw new DomainException($"Property '{filter.SortingProperty}' is not valid as a sort.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled error when trying to sort by '{filter.SortingProperty}': {ex.Message}");
            }
        }

        private static IQueryable<T> OrderInMemory<T>(IQueryable<T> query, Expression<Func<T, object>> expression, SortingType sortingType)
        {
            var func = expression.Compile();

            if (sortingType is SortingType.Descending)
                return query.OrderByDescending(func).AsAsyncQueryable();

            return query.OrderBy(func).AsAsyncQueryable();
        }

        private static bool IsPropertyMapped<T>(Expression<Func<T, object>> expression)
        {
            var propertyName = GetPropertyName(expression);
            var propertyInfo = typeof(T).GetProperty(propertyName);
            var notMappedAttribute = propertyInfo?.GetCustomAttribute<NotMappedAttribute>();

            return notMappedAttribute is null;
        }

        private static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MemberExpression memberExpression)
                    return memberExpression.Member.Name;

                if (unaryExpression.Operand is MethodCallExpression methodCallExpression)
                {
                    if (methodCallExpression.Arguments.FirstOrDefault() is MemberExpression argumentMemberExpression)
                        return argumentMemberExpression.Member.Name;
                }
            }
            else if (expression.Body is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            throw new ArgumentException("The expression does not represent a valid property.");
        }
    }
}