using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using System;
using System.Diagnostics;

namespace SpecDrill.AutomationScopes
{
    public sealed class BenchmarkScope : IDisposable
    {
        private readonly ILogger Log = Infrastructure.Logging.Log.Get<BenchmarkScope>();

        private readonly Stopwatch stopwatch;
        private readonly string description;

        public BenchmarkScope(string description)
        {
            this.description = description;
            stopwatch = new Stopwatch();

            Log.Info(string.Format("Starting Stopwatch for {0}", description));

            stopwatch.Start();
        }

        public TimeSpan Elapsed
        {
            get { return stopwatch.Elapsed; }
        }

        public void Dispose()
        {
            stopwatch.Stop();
            Log.Info(string.Format("Stopped Stopwatch for {0}. Elapsed = {1}", description, stopwatch.Elapsed));
        }
    }
}
