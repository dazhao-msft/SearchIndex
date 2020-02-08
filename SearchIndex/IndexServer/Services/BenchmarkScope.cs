using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace IndexServer.Services
{
    public class BenchmarkScope : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _activity;
        private readonly Stopwatch _stopwatch;

        public BenchmarkScope(ILogger logger, string activity)
        {
            _logger = logger;
            _activity = activity;
            _stopwatch = new Stopwatch();

            _logger.LogInformation($"{_activity} started.");

            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            _logger.LogInformation($"{_activity} completed in {_stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
