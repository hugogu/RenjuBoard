using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Renju.Infrastructure;

namespace RenjuBoard.ViewModels
{
    public class OptionsViewModel : ModelBase
    {
        private bool _showLinesOnBoard = false;
        private bool _showAISteps = true;
        private bool _showPreviewLine = true;
        private bool _showCalibrationLabels = true;
        private bool _isAITimeLimited = true;
        private int _aiStepTimeLimit = 20000;
        private int _aiTimeLimit = 500000;
        private readonly ICommand _cancelCommand;
        private readonly ICommand _saveCommand;

        [InjectionConstructor]
        public OptionsViewModel()
        {
            _cancelCommand = new DelegateCommand(() => Application.Current.Windows.Cast<Window>().Last().DialogResult = false);
            _saveCommand = new DelegateCommand(() => Application.Current.Windows.Cast<Window>().Last().DialogResult = true);
            ShowOptionsCommand = new DelegateCommand(OnShowOptionsCommand);
        }

        public OptionsViewModel(OptionsViewModel vm)
            : this()
        {
            CopyFrom(vm);
        }

        public ICommand ShowOptionsCommand { get; private set; }

        public bool ShowLinesOnBoard
        {
            get { return _showLinesOnBoard; }
            set { SetProperty(ref _showLinesOnBoard, value); }
        }

        public bool ShowAISteps
        {
            get { return _showAISteps; }
            set { SetProperty(ref _showAISteps, value); }
        }

        public bool ShowPreviewLine
        {
            get { return _showPreviewLine; }
            set { SetProperty(ref _showPreviewLine, value); }
        }

        public bool ShowCalibrationLabels
        {
            get { return _showCalibrationLabels; }
            set { SetProperty(ref _showCalibrationLabels, value); }
        }

        public int AIStepTimeLimit
        {
            get { return _aiStepTimeLimit; }
            set { SetProperty(ref _aiStepTimeLimit, value); }
        }

        public int AITimeLimit
        {
            get { return _aiTimeLimit; }
            set { SetProperty(ref _aiTimeLimit, value); }
        }

        public bool IsAITimeLimited
        {
            get { return _isAITimeLimited; }
            set { SetProperty(ref _isAITimeLimited, value); }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public void CopyFrom(OptionsViewModel anotherVM)
        {
            foreach(var property in typeof(OptionsViewModel).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                            .Where(p => p.CanWrite && p.CanRead))
            {
                property.SetValue(this, property.GetValue(anotherVM));
            }
        }

        private void OnShowOptionsCommand()
        {
            var optionsCopy = new OptionsViewModel(this);
            var optionsWindow = new Window()
            {
                Owner = Application.Current.MainWindow,
                Title = "Renju Options",
                Content = optionsCopy,
                MinHeight = 200,
                MinWidth = 300,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (optionsWindow.ShowDialog() == true)
            {
                this.CopyFrom(optionsCopy);
            }
        }
    }
}
