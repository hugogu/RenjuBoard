namespace Renju.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;

    public static class Reflections
    {
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
    }
}
