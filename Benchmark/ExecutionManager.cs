using BenchmarkDotNet.Attributes;
using QueueManager;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class ExecutionManagerBenchmark
    {
        int needTasks = 10000;
        int counter;
        ExecutionManager Manager;
        public ExecutionManagerBenchmark()
        {
            Manager = new ExecutionManager();
        }
        Task Method()
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        }
        [Benchmark]
        public async Task Async()
        {
            counter = 0;
            for (var i = 0; i < needTasks; i++) await Manager.Enqueue(Method);
            await Manager.AwaitQueue();

            //if (needTasks != counter)
            Console.WriteLine($"======================= Executed {counter} times ==============");
        }
    }
}
