namespace Renju.Infrastructure.Execution
{
    using System;

    public class ResourceUsageInformation
    {
        public ResourceUsageInformation(long memory, double cpu)
        {
            Memory = memory;
            CPU = cpu;
            Time = DateTime.Now;
        }

        public DateTime Time { get; private set; }

        public long Memory { get; private set; }

        public double CPU { get; private set; }
    }
}
