namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Utility;
    using Models;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

    public class GamePlayerSetupViewModel : ModelBase
    {
        private readonly Type _playerType;
        private readonly Side _side;

        public ConstructorInfo Constructor { get; private set; }

        public IEnumerable<ResolveOverrideItem> Parameters { get; private set; }

        public IEnumerable<ResolveOverrideItem> Properties { get; private set; }

        public IEnumerable<ResolveOverrideItem> DisplayItems
        {
            get { return Parameters.Concat(Properties); }
        }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public string Name
        {
            get { return _playerType.GetDisplayName(); }
        }

        public IGamePlayer CreatedPlayer
        {
            get
            {
                var resolverOverrides = Parameters.Select(p => new ParameterOverride(p.Name, p.SelectedCandidate) as ResolverOverride)
                    .Concat(Properties.Where(p => !p.IsReadOnly).Select(p => new PropertyOverride(p.Name, p.SelectedCandidate)))
                    .ToArray();

                var player = Container.Resolve(_playerType, resolverOverrides) as IGamePlayer;
                player.Side = _side;
                Container.RegisterInstance(_side.ToString(), player);

                return player;
            }
        }

        public GamePlayerSetupViewModel(Type playerType, Side side)
        {
            Guard.TypeIsAssignable(typeof(IGamePlayer), playerType, "playerType");
            Constructor = playerType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            Debug.Assert(Constructor != null, String.Format("{0} doesn't have a valid constructor.", playerType));
            _side = side;
            _playerType = playerType;
            Parameters = ResolveOverrideItem.BuildParameterModels(Constructor).ToList();
            Properties = ResolveOverrideItem.BuildPropertiesModels(playerType).ToList();
        }
    }
}
