using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Renju.Infrastructure
{
    public static class Reflections
    {
        public static string GetMemberName<T>(this Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null && expression.Body is UnaryExpression)
            {
                memberExpression = (expression.Body as UnaryExpression).Operand as MemberExpression;
            }
            if (memberExpression == null)
                throw new ArgumentException("expression is not a valid member expression", "expression");

            return memberExpression.Member.Name;
        }

        public static void CopyFrom<T>(this T target, T source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (source == null)
                throw new ArgumentNullException("source");

            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                              .Where(p => p.CanWrite && p.CanRead))
            {
                property.SetValue(target, property.GetValue(source));
            }
        }
    }
}
