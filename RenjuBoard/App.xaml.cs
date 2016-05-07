using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;

namespace RenjuBoard
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoadResourceFile(key => key.Contains("-" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName));
            LoadResourceFile(key => key.StartsWith("views"));
            new RenjuBoardBootstrapper().Run();
        }

        private void LoadResourceFile(Func<string, bool> withKeyThat)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            using (var resourceReader = new ResourceReader(stream))
            {
                foreach (var baml in from DictionaryEntry entry in resourceReader
                                     let key = entry.Key as string
                                     where key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase) && withKeyThat(key)
                                     select new { Key = key, Uri = new Uri("/" + assembly.GetName().Name + ";component/" + key.Replace(".baml", ".xaml"), UriKind.Relative) })
                {
                    Trace.WriteLine("Loading resource " + baml.Uri);
                    Resources.MergedDictionaries.Add(LoadComponent(baml.Uri) as ResourceDictionary);
                }
            }
        }
    }
}
