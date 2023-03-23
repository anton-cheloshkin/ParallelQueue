## ParallelQueue

Asynchronous parallel appendable execution library.\
Can await for batched execution

## Example
```c#
var queue = new ParallelQueueExecutor();
Func<Task> myTask = async () =>
{
  // await do something work
  await Task.CompletedTask;
};

queue.DegreeOfParallelism = 1;

queue.Enqueue(myTask);
await queue.EnqueueAsync(myTask);

Enumerable
  .Range(0, 10)
  .AsParallel()
  .WithDegreeOfParallelism(Environment.ProcessorCount)
  .ForAll(i => queue.Enqueue(myTask));


// resize parallel execution size on the fly
queue.DegreeOfParallelism = Environment.ProcessorCount;

queue.Enqueue(() => {
  // do something work
});

await queue.EnqueueAsync(async () => {
  // await do something work
});

// resize parallel execution size on the fly
queue.DegreeOfParallelism = Environment.ProcessorCount / 2;

Enumerable
  .Range(0, 10)
  .AsParallel()
  .WithDegreeOfParallelism(Environment.ProcessorCount)
  .ForAll(async i => await queue.EnqueueAsync(myTask));


queue.OnDone(() => Console.WriteLine("message1"));
await queue.OnDoneAsync(() => Console.WriteLine("message2"));
await queue.OnDoneAsync();
Console.WriteLine("message3");

//output
//message1
//message2
//message3
```

## Benchmark
``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19043.928/21H1/May2021Update)
Intel Core i5-10600 CPU 3.30GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.400
  [Host]     : .NET Core 3.1.18 (CoreCLR 4.700.21.35901, CoreFX 4.700.21.36305), X64 RyuJIT AVX2
  DefaultJob : .NET Core 3.1.18 (CoreCLR 4.700.21.35901, CoreFX 4.700.21.36305), X64 RyuJIT AVX2


```
|         Method |         Arr |        Mean |     Error |    StdDev |        Gen0 |       Gen1 |      Gen2 |  Allocated |
|--------------- |------------ |------------:|----------:|----------:|------------:|-----------:|----------:|-----------:|
|      **Benchmark(300 threads)** |  **Int32[300]** |    **34.57 ms** |  **0.691 ms** |  **1.796 ms** |   **2933.3333** |   **200.0000** |         **-** |   **16.35 MB** |
| BenchmarkAsync(300 threads) |  Int32[300] |    14.46 ms |  0.281 ms |  0.263 ms |   2953.1250 |   687.5000 |         - |   16.35 MB |
|      **Benchmark(500 threads)** |  **Int32[500]** |    **92.19 ms** |  **1.842 ms** |  **4.449 ms** |   **8200.0000** |   **200.0000** |         **-** |   **40.89 MB** |
| BenchmarkAsync(500 threads) |  Int32[500] |    41.81 ms |  0.595 ms |  0.556 ms |   8230.7692 |   230.7692 |         - |   40.89 MB |
|      **Benchmark(1000 threads)** | **Int32[1000]** |   **335.41 ms** | **10.272 ms** | **30.127 ms** |  **32000.0000** |  **8000.0000** |         **-** |   **150.3 MB** |
| BenchmarkAsync(1000 threads) | Int32[1000] |   180.86 ms |  1.918 ms |  1.794 ms |  32333.3333 |  2333.3333 |  666.6667 |  150.29 MB |
|      **Benchmark(3000 threads)** | **Int32[3000]** | **2,383.62 ms** | **46.216 ms** | **36.083 ms** | **276000.0000** | **14000.0000** | **3000.0000** | **1274.57 MB** |
| BenchmarkAsync(3000 threads) | Int32[3000] | 1,795.82 ms |  7.121 ms |  5.947 ms | 280000.0000 |  5000.0000 | 1000.0000 | 1274.58 MB |
