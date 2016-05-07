using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;
using RenjuBoard.Properties;

namespace RenjuBoard.ViewModels
{
    public class OptionsViewModel : ModelBase
    {
        public OptionsViewModel(GameOptions options)
        {
            Options = options;
            CancelCommand = new DelegateCommand(() => Application.Current.Windows.Cast<Window>().Last().DialogResult = false);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            ShowOptionsCommand = new DelegateCommand(OnShowOptionsCommand);
        }

        [Dependency]
        public IEnumerable<IGameRule> Rules { get; set; }

        public GameOptions Options { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        private void OnShowOptionsCommand()
        {
            var optionsCopy = new OptionsViewModel(new GameOptions(Options)) { Rules = this.Rules };
            var optionsWindow = new Window()
            {
                Owner = Application.Current.MainWindow,
                Title = "Renju Options",
                DataContext = optionsCopy,
                MinHeight = 200,
                MinWidth = 300,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                ShowInTaskbar = false,
                Topmost = true,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (optionsWindow.ShowDialog() == true)
            {
                Options.CopyFrom(optionsCopy.Options);
            }
        }

        private void OnSaveCommand()
        {
            Settings.Default.CopyFromObject(Options);
            Settings.Default.Save();
            var optionWindow = Application.Current.Windows.Cast<Window>().Last();
            optionWindow.DialogResult = true;
        }
    }
}
