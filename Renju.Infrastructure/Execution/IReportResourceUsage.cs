namespace Renju.Infrastructure.Execution
{
    using System;

    public interface IReportResourceUsage
    {
        IObservable<ResourceUsageInformation> ResourceUsages { get; }
    }
}
