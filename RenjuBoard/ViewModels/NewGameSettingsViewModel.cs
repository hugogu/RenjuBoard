namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;
    using Prism.Commands;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

    public class NewGameSettingsViewModel : DisposableModelBase
    {
        private Type _blackPlayer;
        private Type _whitePlayer;
        private GamePlayerSetupViewModel _blackPlayerSetupModel;
        private GamePlayerSetupViewModel _whitePlayerSetupModel;

        public NewGameSettingsViewModel(Func<Type, Side, GamePlayerSetupViewModel> createPlayerSetupVM)
        {
            GamePlayers = typeof(IGamePlayer).FindAllImplementations();
            Contract.Assert(GamePlayers.Any(), "No Gameplayer found");
            AutoDispose(this.ObserveProperty(() => BlackPlayerType).Subscribe(e =>
                BlackPlayerBuilder = createPlayerSetupVM(BlackPlayerType, Side.Black)));
            AutoDispose(this.ObserveProperty(() => WhitePlayerType).Subscribe(e =>
                WhitePlayerBuilder = createPlayerSetupVM(WhitePlayerType, Side.White)));

            BlackPlayerType = GamePlayers.First();
            WhitePlayerType = GamePlayers.Skip(1).First();

            SwitchPlayer = new DelegateCommand(() =>
            {
                var temp = BlackPlayerType;
                BlackPlayerType = WhitePlayerType;
                WhitePlayerType = temp;
            });
        }

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

        public ICommand SwitchPlayer { get; private set; }
    }
}
