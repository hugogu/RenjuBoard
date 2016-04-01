using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;

namespace RenjuBoard.ViewModels
{
    public class LogsViewModel : TraceListener
    {
        private readonly ObservableCollection<string> _logs = new ObservableCollection<string>();
        private readonly ICommand _clearLogCommand;

        public LogsViewModel()
        {
            Trace.Listeners.Add(this);
            _clearLogCommand = new DelegateCommand(() => _logs.Clear());
            Trace.WriteLine(typeof(LogsViewModel).Name + " initialized.");
        }

        public IEnumerable<string> Logs
        {
            get { return _logs; }
        }

        public ICommand ClearLogsCommand
        {
            get { return _clearLogCommand; }
        }

        public override void Write(string message)
        {
            AddLogItem(message);
        }

        public override void WriteLine(string message)
        {
            AddLogItem(message);
        }

        private void AddLogItem(string message)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() => _logs.Insert(0, message)), DispatcherPriority.DataBind);
        }
    }
}
