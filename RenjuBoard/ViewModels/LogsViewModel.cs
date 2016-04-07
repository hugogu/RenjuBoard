using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Runju.Infrastructure;

namespace RenjuBoard.ViewModels
{
    public class LogsViewModel : ModelBase
    {
        private string _logText;
        private string _searchKey;
        private readonly ICommand _clearLogCommand;
        private readonly ICommand _searchCommand;

        public LogsViewModel()
        {
            Trace.Listeners.Add(new LogsViewModelAdeptor(this));
            Trace.WriteLine(typeof(LogsViewModel).Name + " initialized.");
            _clearLogCommand = new DelegateCommand(() => LogText = String.Empty);
            _searchCommand = new DelegateCommand<TextBox>(OnSearchCommand);
        }

        private class LogsViewModelAdeptor : TraceListener
        {
            private readonly LogsViewModel _logVM;

            public LogsViewModelAdeptor(LogsViewModel viewModel)
            {
                _logVM = viewModel;
            }

            public override void Write(string message)
            {
                _logVM.PrependMessage(message);
            }

            public override void WriteLine(string message)
            {
                _logVM.PrependMessage(message + Environment.NewLine);
            }
        }

        public string LogText
        {
            get { return _logText; }
            set { SetProperty(ref _logText, value, () => LogText, true); }
        }

        public string SearchKeyword
        {
            get { return _searchKey; }
            set { SetProperty(ref _searchKey, value, () => SearchKeyword); }
        }

        public ICommand ClearLogsCommand
        {
            get { return _clearLogCommand; }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private void OnSearchCommand(TextBox logBox)
        {
            if (String.IsNullOrEmpty(SearchKeyword))
                return;

            var currentIndex = logBox.CaretIndex;
            if (logBox.IsSelectionActive)
                currentIndex = logBox.SelectionStart + logBox.SelectionLength;

            var index = _logText.IndexOf(SearchKeyword, currentIndex);
            if (index >= 0)
            {
                logBox.Select(index, SearchKeyword.Length);
                logBox.Focus();
            }
            else
            {
                MessageBox.Show(String.Format("Can't find {0} after index {1}", SearchKeyword, currentIndex), "Renju Log Search");
            }
        }

        private void PrependMessage(string message)
        {
            AddLogItemAsync(() => LogText = message + LogText);
        }

        private void AddLogItemAsync(Action action)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }
    }
}
