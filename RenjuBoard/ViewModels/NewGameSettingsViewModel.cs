namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

    public class NewGameSettingsViewModel : DisposableModelBase
    {
        private Type _blackPlayer;
        private Type _whitePlayer;
        private GamePlayerSetupViewModel _blackPlayerSetupModel;
        private GamePlayerSetupViewModel _whitePlayerSetupModel;

        public Type BlackPlayerType
        {
            get { return _blackPlayer; }
            set { SetProperty(ref _blackPlayer, value); }
        }

        public Type WhitePlayerType
        {
            get { return _whitePlayer; }
            set { SetProperty(ref _whitePlayer, value); }
        }

        public GamePlayerSetupViewModel BlackPlayerBuilder
        {
            get { return _blackPlayerSetupModel; }
            set { SetProperty(ref _blackPlayerSetupModel, value); }
        }

        public GamePlayerSetupViewModel WhitePlayerBuilder
        {
            get { return _whitePlayerSetupModel; }
            set { SetProperty(ref _whitePlayerSetupModel, value); }
        }

        public IEnumerable<Type> GamePlayers { get; private set; }

        public NewGameSettingsViewModel(IUnityContainer container)
        {
            GamePlayers = typeof(IGamePlayer).FindAllImplementations();
            Debug.Assert(GamePlayers.Any(), "No Gameplayer found");
            AutoDispose(this.ObserveProperty(() => BlackPlayerType).Subscribe(e =>
                BlackPlayerBuilder = new GamePlayerSetupViewModel(BlackPlayerType) { Container = container }));
            AutoDispose(this.ObserveProperty(() => WhitePlayerType).Subscribe(e =>
                WhitePlayerBuilder = new GamePlayerSetupViewModel(WhitePlayerType) { Container = container }));

            BlackPlayerType = GamePlayers.First();
            WhitePlayerType = GamePlayers.Skip(1).First();
        }
    }
}
