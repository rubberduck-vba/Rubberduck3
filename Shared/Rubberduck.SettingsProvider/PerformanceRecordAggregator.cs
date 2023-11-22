using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.SettingsProvider
{
    public class PerformanceRecordAggregator
    {
        private readonly ILogger _logger;
        private readonly RubberduckSettingsProvider _settings;
        private readonly ConcurrentDictionary<string, ConcurrentBag<TimeSpan>> _items = new();
        private readonly ConcurrentDictionary<string, TimeSpan> _totals = new();
        private readonly ConcurrentDictionary<string, DateTime> _firstWrites = new();

        // TODO make these settings
        private static readonly int SmallSampleSize = 10;
        private static readonly int LargeSampleSize = 100;
        private static readonly int MaxSampleSize = 1000;

        public PerformanceRecordAggregator(ILogger<PerformanceRecordAggregator> logger, RubberduckSettingsProvider settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public int Add(PerformanceRecord record)
        {
            if (!_items.TryGetValue(record.Name, out var bag))
            {
                bag = new ConcurrentBag<TimeSpan>();
                _items[record.Name] = bag;
                _totals[record.Name] = TimeSpan.Zero;
                _firstWrites[record.Name] = DateTime.Now;
            }

            bag.Add(record.Elapsed);
            _totals[record.Name] += record.Elapsed;

            OnRecordAdded(record.Name, bag);
            return bag.Count;
        }

        private void OnRecordAdded(string name, ConcurrentBag<TimeSpan> bag)
        {
            var count = bag.Count;
            if (count % LargeSampleSize == 0)
            {
                LogBagSummary(name);
            }
            else if (count % SmallSampleSize == 0)
            {
                LogBagSummary(name);
            }

            if (count >= MaxSampleSize)
            {
                var elapsed = TimedAction.Run(() => { bag.Clear(); });
                var verbosity = _settings.TraceLevel;
                _logger.LogTrace(verbosity, $"Performance data cleared for [{name}] ({count} items). Elapsed: {elapsed}");
            }
        }

        private void LogBagSummary(string name)
        {
            var sampleSize = 0;
            var total = TimeSpan.Zero;
            var average = TimeSpan.Zero;
            var stdev = TimeSpan.Zero;
            var min = TimeSpan.Zero;
            var max = TimeSpan.Zero;
            var median = TimeSpan.Zero;

            var elapsed = TimedAction.Run(() =>
            {
                sampleSize = Count(name);
                total = TotalElapsed(name);
                average = AverageElapsed(name);
                stdev = StandardDeviation(name);
                min = MinElapsed(name);
                max = MaxElapsed(name);
                median = Median(name);
            });

            var verbosity = _settings.TraceLevel;
            _logger.LogTrace(verbosity, $"**PERF[{name}]:{sampleSize} events. Total: {total} Min: {min} Max {max} Average: {average} Median: {median} Std.Dev.: {stdev} (calculations: {elapsed})");
        }

        public int Count(string name) => _items[name].Count;

        public TimeSpan TotalElapsed(string name) => _totals[name];

        public TimeSpan AverageElapsed(string name) => TotalElapsed(name) / Count(name);
        public TimeSpan MinElapsed(string name) => _items[name].Min();
        public TimeSpan MaxElapsed(string name) => _items[name].Max();
        public TimeSpan Median(string name)
        {
            var median = _items[name].Median(e => e.TotalMilliseconds);
            return TimeSpan.FromMilliseconds(median);
        }
        public TimeSpan StandardDeviation(string name)
        {
            var count = Count(name);
            if (count == 0)
            {
                return TimeSpan.Zero;
            }
            var average = AverageElapsed(name).TotalMilliseconds;
            var sumOfSqaredDeltas = _items[name]
                .Select(elapsed => (elapsed.TotalMilliseconds - average) * (elapsed.TotalMilliseconds - average))
                .Sum();
            var stdev = Math.Sqrt(sumOfSqaredDeltas / count);
            return TimeSpan.FromMilliseconds(stdev);
        }
    }

    /// <summary>
    /// Introduction to Algorithms by Cormen et al, 3rd Edition, posted by Shital Shah: https://stackoverflow.com/a/22702269
    /// </summary>
    internal static class StatsExtensions
    {
        /// <summary>
        /// Partitions the given list around a pivot element such that all elements on left of pivot are <= pivot
        /// and the ones at thr right are > pivot. This method can be used for sorting, N-order statistics such as
        /// as median finding algorithms.
        /// Pivot is selected ranodmly if random number generator is supplied else its selected as last element in the list.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
        /// </summary>
        private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
        {
            if (rnd != null)
                list.Swap(end, rnd.Next(start, end + 1));

            var pivot = list[end];
            var lastLow = start - 1;
            for (var i = start; i < end; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                    list.Swap(i, ++lastLow);
            }
            list.Swap(end, ++lastLow);
            return lastLow;
        }

        /// <summary>
        /// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </summary>
        public static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
        {
            return list.NthOrderStatistic(n, 0, list.Count - 1, rnd);
        }
        private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
        {
            while (true)
            {
                var pivotIndex = list.Partition(start, end, rnd);
                if (pivotIndex == n)
                    return list[pivotIndex];

                if (n < pivotIndex)
                    end = pivotIndex - 1;
                else
                    start = pivotIndex + 1;
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            if (i == j)   //This check is not required but Partition function may make many calls so its for perf reason
                return;
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Note: specified list would be mutated in the process.
        /// </summary>
        public static T Median<T>(this IList<T> list) where T : IComparable<T>
        {
            return list.NthOrderStatistic((list.Count - 1) / 2);
        }

        public static double Median<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var list = sequence.Select(getValue).ToList();
            var mid = (list.Count - 1) / 2;
            return list.NthOrderStatistic(mid);
        }
    }
}
