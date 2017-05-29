namespace RenjuBoard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using Renju.Infrastructure;
    using Renju.Infrastructure.Execution;

    public class AIResourceUsageViewModel : DisposableModelBase
    {
        private readonly ICollection<ResourceUsageInformation> _usageInfo = new ObservableCollection<ResourceUsageInformation>();

        public AIResourceUsageViewModel(IReportResourceUsage usageReportor)
        {
            AutoDispose(usageReportor.ResourceUsages.ObserveOnDispatcher().Subscribe(info => _usageInfo.Add(info)));
        }

        public IEnumerable<ResourceUsageInformation> UsageInfo
        {
            get { return _usageInfo; }
        }
    }
}
