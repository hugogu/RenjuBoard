namespace RenjuBoard.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Practices.Unity;
    using Prism.Commands;
    using Properties;
    using Reflection4Net.Extensions;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

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
            var optionsCopy = new OptionsViewModel(new GameOptions().CopyFrom(Options)) { Rules = this.Rules };
            optionsCopy.CreateViewModelDialog("Renju Options")
                       .WithMinSize(300, 200)
                       .OnOKDo(() => Options.CopyFrom(optionsCopy.Options))
                       .ShowDialog();
        }

        private void OnSaveCommand()
        {
            Settings.Default.MapFrom(Options);
            Settings.Default.Save();
            var optionWindow = Application.Current.Windows.Cast<Window>().Last();
            optionWindow.DialogResult = true;
        }
    }
}
