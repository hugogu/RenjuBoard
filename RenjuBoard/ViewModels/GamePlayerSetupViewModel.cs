﻿namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Model;

    public class GamePlayerSetupViewModel : ModelBase
    {
        private readonly Type _playerType;

        public ConstructorInfo Constructor { get; private set; }

        public IEnumerable<ParameterModel> Parameters { get; private set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public string Name
        {
            get { return _playerType.Name; }
        }

        public IGamePlayer CreatedPlayer
        {
            get { return Container.Resolve(_playerType, Parameters.Select(p => new ParameterOverride(p.Name, p.SelectedCandidate)).ToArray()) as IGamePlayer; }
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
            Parameters = ParameterModel.BuildParameterModels(Constructor).ToList();
        }
    }

    public class ParameterModel : ModelBase
    {
        private readonly ParameterInfo _parameterInfo;
        private object _value;

        public ParameterModel(ParameterInfo parameter, IEnumerable<object> candidates)
        {
            _parameterInfo = parameter;
            Candidates = candidates;
        }

        public string Name
        {
            get { return _parameterInfo.Name; }
        }

        public bool HasCandidates
        {
            get { return Candidates.Any(); }
        }

        public object SelectedCandidate
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public IEnumerable<object> Candidates { get; private set; }

        public static IEnumerable<ParameterModel> BuildParameterModels(ConstructorInfo constructorInfo)
        {
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                if (parameterInfo.ParameterType == typeof(string))
                {
                    if (parameterInfo.Name.EndsWith("file", StringComparison.OrdinalIgnoreCase))
                        yield return new ParameterModel(parameterInfo, Directory.EnumerateFiles(".", "*.exe", SearchOption.AllDirectories));
                    else
                        yield return new ParameterModel(parameterInfo, new object[0]) { SelectedCandidate = String.Empty };
                }
                else if (parameterInfo.ParameterType.IsEnum)
                {
                    yield return new ParameterModel(parameterInfo, Enum.GetValues(parameterInfo.ParameterType).Cast<object>());
                }
                else if (parameterInfo.ParameterType.IsInterface)
                {
                    yield return new ParameterModel(parameterInfo, parameterInfo.ParameterType.FindAllImplementations());
                }
                else
                {
                    throw new NotSupportedException(String.Format("Parameter of type {0} hasn't been support while constructing a game player", parameterInfo.ParameterType));
                }
            }
        }
    }
}
