namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;
    using RenjuBoard.Models;

    public class GamePlayerSetupViewModel : ModelBase
    {
        private readonly Type _playerType;

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
                var resolverOverrides = Parameters.Select(p => new ParameterOverride(p.Name, p.SelectedCandidate) as ResolverOverride).Concat(
                    Properties.Where(p => !p.IsReadOnly).Select(p => new PropertyOverride(p.Name, p.SelectedCandidate))).ToArray();

                return Container.Resolve(_playerType, resolverOverrides) as IGamePlayer;
            }
        }

        public GamePlayerSetupViewModel(Type playerType)
        {
            if (!typeof(IGamePlayer).IsAssignableFrom(playerType))
            {
                throw new ArgumentException(String.Format("{0} is not valid game player type", playerType));
            }
            Constructor = playerType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            if (Constructor == null)
            {
                throw new ArgumentException(String.Format("{0} doesn't have a valid constructor.", playerType), "playerType");
            }
            _playerType = playerType;
            Parameters = ResolveOverrideItem.BuildParameterModels(Constructor).ToList();
            Properties = ResolveOverrideItem.BuildPropertiesModels(playerType).ToList();
        }
    }
}
