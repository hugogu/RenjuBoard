namespace RenjuBoard.ViewModels
{
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Practices.Unity;
    using Microsoft.Win32;
    using Prism.Commands;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

    public class SaveAndLoadViewModel : ModelBase
    {
        public SaveAndLoadViewModel()
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            LoadCommand = new DelegateCommand(OnLoadCommand);
        }

        [Dependency]
        public BoardRecorder BoardRecorder { get; internal set; }

        [Dependency]
        public IGameBoard<IReadOnlyBoardPoint> GameBoard { get; internal set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        private async void OnLoadCommand()
        {
            var loadFile = AskForLoadingLocation();
            if (loadFile != null)
            {
                BoardRecorder.ClearGameBoard();
                using (var streamReader = new StreamReader(File.OpenRead(loadFile)))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(PieceDrop));
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync().ConfigureAwait(false);
                        var drop = converter.ConvertFromString(line) as PieceDrop;
                        GameBoard.Drop(drop, OperatorType.Loading);
                    }
                }
            }
        }

        private void OnSaveCommand()
        {
            var saveFile = AskForSavingLocation();
            if (saveFile != null)
            {
                using (var streamWriter = new StreamWriter(File.OpenWrite(saveFile)))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(PieceDrop));
                    foreach (var drop in BoardRecorder.Drops.Concat(BoardRecorder.RedoDrops))
                    {
                        streamWriter.WriteLine(converter.ConvertToString(drop));
                    }
                }
            }
        }

        private string AskForLoadingLocation()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ValidateNames = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Renju Game (*.renju) |*.renju| All Files |*.*";
            openFileDialog.Title = "Select a file for Renju Game";

            return openFileDialog.ShowDialog(Application.Current.MainWindow) == true ?
                   openFileDialog.FileName : null;
        }

        private string AskForSavingLocation()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ValidateNames = true;
            saveFileDialog.Filter = "Renju Game (*.renju) |*.renju| All Files |*.*";
            saveFileDialog.Title = "Select a file for Renju Game";

            return saveFileDialog.ShowDialog(Application.Current.MainWindow) == true ?
                   saveFileDialog.FileName : null;
        }
    }
}
