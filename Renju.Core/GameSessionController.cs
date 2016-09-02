namespace Renju.Core
{
    using System;
    using System.Windows;
    using Infrastructure.AI;
    using Infrastructure.Events;
    using Infrastructure.Execution;
    using Microsoft.Practices.Unity;
    using Prism.Events;

    public class GameSessionController<TMainVM>
    {
        private IUnityContainer _currentGameContainer;
        private IEventAggregator _eventAggregator;

        [Dependency]
        public IUnityContainer ApplicationContainer { get; set; }

        public GameSessionController(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<StartNewGameEvent>().Subscribe(OnNewGameEvent, ThreadOption.UIThread, true);
        }

        public void StartNewGame()
        {
            _eventAggregator.GetEvent<StartNewGameEvent>().Publish(NewGameOptions.Default);
        }

        private void OnNewGameEvent(NewGameOptions options)
        {
            RenewChildContainerForGame(options);
            Application.Current.MainWindow.DataContext = _currentGameContainer.Resolve<TMainVM>();
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
