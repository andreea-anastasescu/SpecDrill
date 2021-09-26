using Microsoft.Extensions.Logging;
using SpecDrill.Infrastructure;
using System;
using System.Diagnostics;

namespace SpecDrill.AutomationScopes
{
    public sealed class BenchmarkScope : IDisposable
    {
        private readonly ILogger Logger = DI.GetLogger<BenchmarkScope>();

        private readonly Stopwatch stopwatch;
        private readonly string description;

        public BenchmarkScope(string description)
        {
            this.description = description;
            stopwatch = new Stopwatch();

            Logger.LogInformation(string.Format("Starting Stopwatch for {0}", description));

            stopwatch.Start();
        }

        public TimeSpan Elapsed
        {
            get { return stopwatch.Elapsed; }
        }

        public void Dispose()
        {
            stopwatch.Stop();
            Logger.LogInformation(string.Format("Stopped Stopwatch for {0}. Elapsed = {1}", description, stopwatch.Elapsed));
        }
    }
}
