using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Text;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Application performance monitoring and metrics service
    /// </summary>
    public class PerformanceMonitoringService
    {
        private readonly ILogger<PerformanceMonitoringService> _logger;
        private readonly Dictionary<string, List<double>> _metrics;
        private readonly object _lockObject = new();

        public PerformanceMonitoringService(ILogger<PerformanceMonitoringService> logger)
        {
            _logger = logger;
            _metrics = new Dictionary<string, List<double>>();
        }

        /// <summary>
        /// Record execution time for an operation
        /// </summary>
        public void RecordExecutionTime(string operationName, double milliseconds)
        {
            lock (_lockObject)
            {
                if (!_metrics.ContainsKey(operationName))
                {
                    _metrics[operationName] = new List<double>();
                }

                _metrics[operationName].Add(milliseconds);

                // Keep only the last 100 measurements
                if (_metrics[operationName].Count > 100)
                {
                    _metrics[operationName].RemoveAt(0);
                }
            }

            _logger.LogDebug("Operation {OperationName} completed in {Duration}ms", 
                operationName, milliseconds);
        }

        /// <summary>
        /// Get performance statistics for an operation
        /// </summary>
        public PerformanceStats GetPerformanceStats(string operationName)
        {
            lock (_lockObject)
            {
                if (!_metrics.ContainsKey(operationName) || !_metrics[operationName].Any())
                {
                    return new PerformanceStats
                    {
                        OperationName = operationName,
                        SampleCount = 0
                    };
                }

                var measurements = _metrics[operationName];
                return new PerformanceStats
                {
                    OperationName = operationName,
                    SampleCount = measurements.Count,
                    AverageMs = measurements.Average(),
                    MinMs = measurements.Min(),
                    MaxMs = measurements.Max(),
                    MedianMs = GetMedian(measurements),
                    P95Ms = GetPercentile(measurements, 0.95),
                    P99Ms = GetPercentile(measurements, 0.99)
                };
            }
        }

        /// <summary>
        /// Get all performance metrics
        /// </summary>
        public Dictionary<string, PerformanceStats> GetAllMetrics()
        {
            var result = new Dictionary<string, PerformanceStats>();
            
            lock (_lockObject)
            {
                foreach (var operationName in _metrics.Keys)
                {
                    result[operationName] = GetPerformanceStats(operationName);
                }
            }

            return result;
        }

        /// <summary>
        /// Clear all metrics
        /// </summary>
        public void ClearMetrics()
        {
            lock (_lockObject)
            {
                _metrics.Clear();
            }
            _logger.LogInformation("Performance metrics cleared");
        }

        private static double GetMedian(List<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            var count = sorted.Count;
            
            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            else
            {
                return sorted[count / 2];
            }
        }

        private static double GetPercentile(List<double> values, double percentile)
        {
            var sorted = values.OrderBy(x => x).ToList();
            var index = (int)Math.Ceiling(sorted.Count * percentile) - 1;
            return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
        }
    }

    /// <summary>
    /// Performance statistics data model
    /// </summary>
    public class PerformanceStats
    {
        public string OperationName { get; set; } = string.Empty;
        public int SampleCount { get; set; }
        public double AverageMs { get; set; }
        public double MinMs { get; set; }
        public double MaxMs { get; set; }
        public double MedianMs { get; set; }
        public double P95Ms { get; set; }
        public double P99Ms { get; set; }
    }

    /// <summary>
    /// Extension methods for measuring performance
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Execute an operation and measure its performance
        /// </summary>
        public static async Task<T> MeasureAsync<T>(
            this PerformanceMonitoringService monitor,
            string operationName,
            Func<Task<T>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await operation();
                stopwatch.Stop();
                monitor.RecordExecutionTime(operationName, stopwatch.Elapsed.TotalMilliseconds);
                return result;
            }
            catch
            {
                stopwatch.Stop();
                monitor.RecordExecutionTime($"{operationName}_Error", stopwatch.Elapsed.TotalMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Execute an operation and measure its performance (synchronous version)
        /// </summary>
        public static T Measure<T>(
            this PerformanceMonitoringService monitor,
            string operationName,
            Func<T> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = operation();
                stopwatch.Stop();
                monitor.RecordExecutionTime(operationName, stopwatch.Elapsed.TotalMilliseconds);
                return result;
            }
            catch
            {
                stopwatch.Stop();
                monitor.RecordExecutionTime($"{operationName}_Error", stopwatch.Elapsed.TotalMilliseconds);
                throw;
            }
        }
    }
}