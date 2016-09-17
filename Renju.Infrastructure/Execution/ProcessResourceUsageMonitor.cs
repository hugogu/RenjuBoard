namespace Renju.Infrastructure.Execution
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Infrastructure;

    public class ProcessResourceUsageMonitor : DisposableModelBase
    {
        private readonly ISubject<ResourceUsageInformation> _usageInfo = new ReplaySubject<ResourceUsageInformation>();

        public void Monitor(Process process, TimeSpan monitorInterval)
        {
            var totalCPUTime = Observable.Interval(monitorInterval).Select(i => new { Time = DateTime.Now, CPU = process.TotalProcessorTime });
            var totalMemory = Observable.Interval(monitorInterval).Select(i => process.VirtualMemorySize64);
            var cpuUsage = totalCPUTime.Deltas((t1, t2) => (t2.CPU - t1.CPU).TotalMilliseconds / (t2.Time - t1.Time).TotalMilliseconds);
            AutoDispose(totalMemory.Skip(1).Zip(cpuUsage, (mem, cpu) => new ResourceUsageInformation(mem, cpu)).Subscribe(_usageInfo));
        }

        public IObservable<ResourceUsageInformation> UsageInfo
        {
            get { return _usageInfo; }
        }
    }
}
