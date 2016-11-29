namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
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
        public Func<Type, ResolverOverride[], IGamePlayer> CreateGamePlayer { get; set; }

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
                    .Concat(new[] { new PropertyOverride("Side", _side) })
                    .ToArray();

                return CreateGamePlayer(_playerType, resolverOverrides);
            }
        }

        public GamePlayerSetupViewModel(Type playerType, Side side)
        {
            Contract.Requires(playerType != null);
            Contract.Requires(typeof(IGamePlayer).IsAssignableFrom(playerType));
            Constructor = playerType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            Contract.Assert(Constructor != null, "playerType doesn't have a valid constructor.");
            _side = side;
            _playerType = playerType;
            Parameters = ResolveOverrideItem.BuildParameterModels(Constructor).ToList();
            Properties = ResolveOverrideItem.BuildPropertiesModels(playerType).ToList();
        }
    }
}
