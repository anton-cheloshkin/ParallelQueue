using BenchmarkDotNet.Attributes;
using ParallelQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class ParallelQueueBenchmark
    {
        static int[] Lengths = new int[] { 300, 500, 1000, 3000 };
        ParallelQueueExecutor Executor;
        public IEnumerable<int[]> Values()
        {
            var rnd = new Random();
            foreach (var length in Lengths)
                yield return Enumerable
                    .Range(0, length)
                    .Select(i => rnd.Next(0, length))
                    .ToArray();
        }
        [ParamsSource(nameof(Values))]
        public int[] Arr;
        [GlobalSetup]
        public void Setup()
        {
            Executor = new ParallelQueueExecutor();
        }
        Task Sort()
        {
            Arr.OrderBy(r => r).All(r => true);
            return Task.CompletedTask;
        }
        [Benchmark]
        public async Task Benchmark()
        {
            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Enumerable
                    .Range(0, Arr.Length)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                    .ForAll(i => Executor.Enqueue(Sort));

                await Executor.OnDoneAsync();
            }
        }
        [Benchmark]
        public async Task BenchmarkAsync()
        {
            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Enumerable
                    .Range(0, Arr.Length)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                    .ForAll(async i => await Executor.EnqueueAsync(Sort));

                await Executor.OnDoneAsync();
            }
        }
    }
}
