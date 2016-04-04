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
        private bool lastMessageHasNewLine = true;

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
            if (_logs.Count == 0)
            {
                WriteLine(message);
            }
            else
            {
                if (lastMessageHasNewLine)
                    AddLogItem(() => _logs.Insert(0, message));
                else
                    AddLogItem(() => _logs[_logs.Count - 1] += message);
            }
            lastMessageHasNewLine = false;
        }

        public override void WriteLine(string message)
        {
            lastMessageHasNewLine = true;
            AddLogItem(() => _logs.Insert(0, message));
        }

        private void AddLogItem(Action action)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }
    }
}
