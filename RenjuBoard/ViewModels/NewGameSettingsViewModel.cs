namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
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

        public Type BlackPlayer
        {
            get { return _blackPlayer; }
            set { SetProperty(ref _blackPlayer, value); }
        }

        public Type WhitePlayer
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
            GamePlayers = typeof(IGamePlayer).FindAllImplementations().Concat(new Type[] { null });
            AutoDispose(this.ObserveProperty(() => BlackPlayer).Subscribe(e =>
                BlackPlayerBuilder = new GamePlayerSetupViewModel(BlackPlayer) { Container = container }));
            AutoDispose(this.ObserveProperty(() => WhitePlayer).Subscribe(e =>
                WhitePlayerBuilder = new GamePlayerSetupViewModel(WhitePlayer) { Container = container }));

            BlackPlayer = GamePlayers.First();
            WhitePlayer = GamePlayers.First();
        }
    }
}
