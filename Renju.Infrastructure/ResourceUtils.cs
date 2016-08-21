namespace Renju.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Windows;

    public static class ResourceUtils
    {
        public static IEnumerable<ResourceDictionary> TryFindResourceDictionaries(this Assembly assembly, Func<string, bool> keyMatch)
        {
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            using (var resourceReader = new ResourceReader(stream))
            {
                foreach (var baml in from DictionaryEntry entry in resourceReader
                                     let key = entry.Key as string
                                     where key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase) && keyMatch(key)
                                     select new { Key = key, Uri = new Uri("/" + assembly.GetName().Name + ";component/" + key.Replace(".baml", ".xaml"), UriKind.Relative) })
                {
                    yield return Application.LoadComponent(baml.Uri) as ResourceDictionary;
                }
            }
        }

        public static void LoadResourceFileThat(this ResourceDictionary applicationResources, Func<string, bool> withKeyThat)
        {
            foreach (var resourceDictionary in Assembly.GetCallingAssembly().TryFindResourceDictionaries(withKeyThat))
            {
                Trace.WriteLine("Loading resource " + resourceDictionary.Source);
                applicationResources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}
