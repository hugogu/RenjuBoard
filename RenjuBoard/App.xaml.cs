using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace RenjuBoard
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
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

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var error = String.Format("An unhnalded exception was thrown, do you want to keep RenJu running? \r\n\r\n {0}", e.Exception.Message);
            if (MessageBox.Show(error, "Rejun Exception", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
        }
    }
}
