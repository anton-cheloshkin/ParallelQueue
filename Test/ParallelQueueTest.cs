using ParallelQueue;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class ParallelQueueTest
    {
        int[] Arr;
        public ParallelQueueTest()
        {
            var rnd = new Random();
            Arr = Enumerable.Range(0, 1).Select(r => rnd.Next(0, 10000)).ToArray();
        }
        int counter = 0;
        Task TestTask()
        {
            Arr.OrderBy(r => r).Average();
            Interlocked.Increment(ref counter);

            return Task.CompletedTask;
        }
        public async Task Test()
        {
            var stop = new Stopwatch();
            stop.Start();

            var queue = new ParallelQueueExecutor();
            var countExpected = 0;
            var countProcessed = 0;

            for (var i = 0; i < 1000; i++)
            {

                Enumerable
                    .Range(0, i)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .ForAll(i => queue.Enqueue(TestTask));

                queue.OnDone(() =>
                {
                    if (i != counter) Console.WriteLine($"Expected {i}, processed {counter}");
                });
                await queue.OnDoneAsync(() =>
                {
                    if (i != counter) Console.WriteLine($"Expected {i}, processed {counter}");
                });
                await queue.OnDoneAsync();

                countExpected += i;
                countProcessed += counter;

                counter = 0;
            }

            stop.Stop();

            var message = $"Expected total: {countExpected}, Processed total: {countProcessed} in {stop.ElapsedMilliseconds}ms";
            Console.WriteLine(message);

            if (countExpected != countProcessed)
            {
                throw new Exception(message);
            }
        }
        public async Task TestAsync()
        {
            var stop = new Stopwatch();
            stop.Start();

            var queue = new ParallelQueueExecutor();
            var countExpected = 0;
            var countProcessed = 0;

            for (var i = 0; i < 1000; i++)
            {

                Enumerable
                    .Range(0, i)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .ForAll(async i => await queue.EnqueueAsync(TestTask));

                queue.OnDone(() =>
                {
                    if (i != counter) Console.WriteLine($"Expected {i}, processed {counter}");
                });
                await queue.OnDoneAsync(() =>
                {
                    if (i != counter) Console.WriteLine($"Expected {i}, processed {counter}");
                });
                await queue.OnDoneAsync();

                countExpected += i;
                countProcessed += counter;

                counter = 0;
            }

            stop.Stop();

            var message = $"Expected total: {countExpected}, Processed total: {countProcessed} in {stop.ElapsedMilliseconds}ms";
            Console.WriteLine(message);
            if (countExpected != countProcessed)
            {
                throw new Exception(message);
            }
        }
    }
}
