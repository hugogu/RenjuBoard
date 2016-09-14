namespace Renju.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;

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

        public static T CopyFrom<T>(this T target, T source)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(source, "source");

            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                              .Where(p => p.CanWrite && p.CanRead))
            {
                property.SetValue(target, property.GetValue(source));
            }

            return target;
        }

        public static T CopyFromObject<T>(this T target, object source)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(source, "source");

            foreach (var copyFrom in from targetProperty in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     where targetProperty.CanWrite
                                     let sourceProperty = source.GetType().GetProperty(targetProperty.Name, BindingFlags.Instance | BindingFlags.Public)
                                     where sourceProperty != null && sourceProperty.CanRead
                                     select CreatePropertySetter<T>(sourceProperty, targetProperty))
            {
                copyFrom(target, source);
            }

            return target;
        }

        public static IEnumerable<Type> FindAllImplementations(this Type baseType)
        {
            foreach (Type type in from t in AllClasses.FromAssembliesInBasePath()
                                  where baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract
                                  select t)
            {
                yield return type;
            }
        }

        public static string GetDisplayName(this Type type)
        {
            var nameAttribute = type.GetCustomAttribute<DisplayNameAttribute>();

            return nameAttribute == null ? type.Name : nameAttribute.DisplayName;
        }

        public static string GetDisplayName(this ParameterInfo parameterInfo)
        {
            var descriptionAttribute = parameterInfo.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute == null ? parameterInfo.Name : descriptionAttribute.Description;
        }

        public static string GetTitle(this Assembly assembly)
        {
            return assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        public static string GetCompany(this Assembly assembly)
        {
            return assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
        }

        public static CultureInfo GetCultureInfo(this Assembly assembly)
        {
            return CultureInfo.GetCultureInfo(assembly.GetCustomAttribute<AssemblyCultureAttribute>().Culture);
        }

        private static Action<T, object> CreatePropertySetter<T>(PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            return (t, s) =>
            {
                try
                {
                    var sourceValue = sourceProperty.GetValue(s);
                    var targetValue = targetProperty.GetValue(t);
                    if (!Equals(sourceValue, targetValue))
                    {
                        Trace.WriteLine(String.Format("Change Property {0} from {1} to {2}", targetProperty.Name, targetValue, sourceValue));
                        targetProperty.SetValue(t, sourceValue);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(String.Format("Failed to set property {0} because " + ex.Message, targetProperty.Name));
                }
            };
        }
    }
}
