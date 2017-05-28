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
        public Action<IUnityContainer>[] GameContainerRegistrators { get; set; }

        [Dependency("GameSession")]
        public Func<IUnityContainer> CreateGameContainer { get; set; }

        public void StartNewGame()
        {
            RenewChildContainerForGame();
            var newGameViewModel = _currentGameContainer.Resolve<NewGameSettingsViewModel>();
            newGameViewModel.CreateViewModelDialog("Start New Game").ShowDialog();
            var blackplayer = newGameViewModel.BlackPlayerBuilder.CreatedPlayer;
            var whiteplayer = newGameViewModel.WhitePlayerBuilder.CreatedPlayer;
            Debug.Assert(blackplayer != whiteplayer);
            Debug.Assert(blackplayer.Side == Side.Black);
            Debug.Assert(whiteplayer.Side == Side.White);
            ShowResourceMonitorForPlayer(blackplayer);
            ShowResourceMonitorForPlayer(whiteplayer);
            Application.Current.MainWindow.DataContext = _currentGameContainer.Resolve<MainWindowViewModel>();
            whiteplayer.PlayOn(_currentGameContainer.Resolve<IBoardMonitor>());
            blackplayer.PlayOn(_currentGameContainer.Resolve<IBoardMonitor>());
            _currentGameContainer.Resolve<IGameBoard<IReadOnlyBoardPoint>>().BeginGame();
        }

        private void ShowResourceMonitorForPlayer(IGamePlayer player)
        {
            if (player is IReportResourceUsage)
            {
                new AIResourceUsageViewModel(player as IReportResourceUsage)
                    .CreateViewModelDialog("Resource Monitor - " + player.Name, ResizeMode.CanResizeWithGrip)
                    .WithSize(500, 400)
                    .Show();
            }
        }

        private void RenewChildContainerForGame()
        {
            _currentGameContainer?.Dispose();
            _currentGameContainer = CreateGameContainer();
            Debug.Assert(_currentGameContainer.Parent != null);
            Array.ForEach(GameContainerRegistrators, register => register(_currentGameContainer));
            var options = _currentGameContainer.Resolve<NewGameOptions>();
            _currentGameContainer.RegisterInstance(options);
            _currentGameContainer.RegisterInstance(BoardPoint.CreateIndexBasedFactory(options.BoardSize));
            var ai = _currentGameContainer.Resolve<IDropResolver>();
            _currentGameContainer.RegisterInstance<IReportExecutionStatus>(nameof(ai), ai);
        }
    }
}
