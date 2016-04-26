using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using Renju.Infrastructure;

namespace RenjuBoard.ViewModels
{
    public class LogsViewModel : ModelBase
    {
        private string _logText;
        private string _searchKey;

        public LogsViewModel()
        {
            Trace.Listeners.Add(new LogsViewModelAdeptor(this));
            Trace.WriteLine(typeof(LogsViewModel).Name + " initialized.");
            ClearLogsCommand = new DelegateCommand(() => LogText = String.Empty);
            SearchCommand = new DelegateCommand<TextBox>(OnSearchCommand);
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
            set { SetProperty(ref _logText, value, true); }
        }

        public string SearchKeyword
        {
            get { return _searchKey; }
            set { SetProperty(ref _searchKey, value); }
        }

        public ICommand ClearLogsCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

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
            RunInDispatcher(() => LogText = String.Format("[{0}] {1}", DateTime.Now, message + LogText));
        }
    }
}
