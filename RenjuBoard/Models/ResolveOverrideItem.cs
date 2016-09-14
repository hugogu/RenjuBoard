namespace RenjuBoard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity.Utility;
    using Renju.Infrastructure;

    public class ResolveOverrideItem : ModelBase
    {
        private object _value;

        public ResolveOverrideItem(string name, string displayName, IEnumerable<object> candidates, bool isReadOnly = false)
        {
            Guard.ArgumentNotNull(name, "name");
            Guard.ArgumentNotNull(candidates, "candidates");
            if (isReadOnly && candidates.Any())
            {
                throw new ArgumentException("Readonly property shouldn't have any value candidates. ", "candidates");
            }

            Name = name;
            DisplayName = displayName;
            IsReadOnly = isReadOnly;
            Candidates = candidates;
            if (!Candidates.Any())
            {
                SelectedCandidate = String.Empty;
            }
        }

        public bool IsReadOnly { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public bool HasCandidates
        {
            get { return Candidates.Any(); }
        }

        public object SelectedCandidate
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public IEnumerable<object> Candidates { get; private set; }

        public static IEnumerable<ResolveOverrideItem> BuildPropertiesModels(Type type)
        {
            return from property in type.GetProperties()
                   let displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>(true)
                   where displayNameAttribute != null
                   let readonlyAttribute = property.GetCustomAttribute<ReadOnlyAttribute>(true)
                   let isReadOnly = readonlyAttribute == null ? false : readonlyAttribute.IsReadOnly
                   let candidates = BuildCandidateValues(property.PropertyType, property.Name)
                   select new ResolveOverrideItem(property.Name, displayNameAttribute.DisplayName, candidates, isReadOnly);
        }

        public static IEnumerable<ResolveOverrideItem> BuildParameterModels(ConstructorInfo constructorInfo)
        {
            return from parameter in constructorInfo.GetParameters()
                   let candidates = BuildCandidateValues(parameter.ParameterType, parameter.Name)
                   select new ResolveOverrideItem(parameter.Name, parameter.GetDisplayName(), candidates);
        }

        public static IEnumerable<object> BuildCandidateValues(Type dataType, string valueName)
        {
            if (dataType == typeof(string))
            {
                if (valueName.EndsWith("file", StringComparison.OrdinalIgnoreCase))
                    return Directory.EnumerateFiles(".", "*.exe", SearchOption.AllDirectories);
                else
                    return new object[0];
            }
            else if (dataType.IsEnum)
            {
                return Enum.GetValues(dataType).Cast<object>();
            }
            else if (dataType.IsInterface)
            {
                return dataType.FindAllImplementations();
            }
            else
            {
                throw new NotSupportedException(String.Format("Parameter of type {0} hasn't been support while constructing a game player", dataType));
            }
        }
    }
}
