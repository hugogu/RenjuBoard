namespace RenjuBoard
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Microsoft.Practices.Unity;
    using Renju.Core;
    using Renju.Infrastructure;
    using Renju.Infrastructure.AI;
    using Renju.Infrastructure.Execution;
    using Renju.Infrastructure.Model;
    using Renju.Infrastructure.Protocols;
    using ViewModels;

    public class GameSessionController : ModelBase
    {
        private IUnityContainer _currentGameContainer;

        [Dependency]
        public IUnityContainer ApplicationContainer { get; set; }

        public void StartNewGame()
        {
            RenewChildContainerForGame(NewGameOptions.Default);

            var newGameViewModel = _currentGameContainer.Resolve<NewGameSettingsViewModel>();
            Debug.Assert(newGameViewModel.WhitePlayerBuilder.Container == _currentGameContainer);
            newGameViewModel.CreateViewModelDialog("Start New Game").ShowDialog();
            var blackplayer = newGameViewModel.BlackPlayerBuilder.CreatedPlayer;
            var whiteplayer = newGameViewModel.WhitePlayerBuilder.CreatedPlayer;
            Debug.Assert(blackplayer.Side == Side.Black);
            Debug.Assert(whiteplayer.Side == Side.White);
            Application.Current.MainWindow.DataContext = _currentGameContainer.Resolve<MainWindowViewModel>();
            whiteplayer.PlayOn(_currentGameContainer.Resolve<IBoardMonitor>());
            blackplayer.PlayOn(_currentGameContainer.Resolve<IBoardMonitor>());
            _currentGameContainer.Resolve<IGameBoard<IReadOnlyBoardPoint>>().BeginGame();
        }

        private void RenewChildContainerForGame(NewGameOptions options)
        {
            if (_currentGameContainer != null)
                _currentGameContainer.Dispose();
            _currentGameContainer = ApplicationContainer.CreateChildContainer();
            RegistGameSessionDependencies(_currentGameContainer);
            if (options == NewGameOptions.Default)
                options = _currentGameContainer.Resolve<NewGameOptions>();
            _currentGameContainer.RegisterInstance(options);
            _currentGameContainer.RegisterInstance(BoardPoint.CreateIndexBasedFactory(options.BoardSize));
            var ai = _currentGameContainer.Resolve<IDropResolver>();
            _currentGameContainer.RegisterInstance<IReportExecutionStatus>("ai", ai);
        }

        private void RegistGameSessionDependencies(IUnityContainer container)
        {
            foreach (var containerRegister in ApplicationContainer.ResolveAll<Action<IUnityContainer>>())
            {
                containerRegister(container);
            }
        }
    }
}
