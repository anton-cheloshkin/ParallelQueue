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
  .ForAll(async i => await queue.Enqueue(myTask));


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
|         Method |         Arr |        Mean |     Error |     StdDev |      Median |        Gen0 |       Gen1 |      Gen2 |  Allocated |
|--------------- |------------ |------------:|----------:|-----------:|------------:|------------:|-----------:|----------:|-----------:|
|      **Benchmark(300 threads)** |  **Int32[300]** |    **34.74 ms** |  **0.695 ms** |   **1.936 ms** |    **34.11 ms** |   **2923.0769** |   **153.8462** |         **-** |   **16.35 MB** |
| BenchmarkAsync(300 threads) |  Int32[300] |    17.96 ms |  0.358 ms |   0.886 ms |    17.88 ms |   2968.7500 |   250.0000 |         - |   16.36 MB |
|      **Benchmark(500 threads)** |  **Int32[500]** |    **93.57 ms** |  **2.192 ms** |   **6.394 ms** |    **92.72 ms** |   **8250.0000** |   **750.0000** |         **-** |   **40.89 MB** |
| BenchmarkAsync(500 threads) |  Int32[500] |    43.84 ms |  0.819 ms |   0.841 ms |    43.84 ms |   8181.8182 |  1181.8182 |         - |    40.9 MB |
|      **Benchmark(1000 threads)** | **Int32[1000]** |   **331.47 ms** | **10.107 ms** |  **29.643 ms** |   **329.38 ms** |  **32000.0000** |  **8000.0000** |         **-** |   **150.3 MB** |
| BenchmarkAsync(1000 threads) | Int32[1000] |   193.05 ms |  5.286 ms |  15.503 ms |   190.04 ms |  32000.0000 |  8000.0000 |         - |   150.3 MB |
|      **Benchmark(3000 threads)** | **Int32[3000]** | **2,372.69 ms** | **47.389 ms** | **107.929 ms** | **2,366.58 ms** | **278000.0000** | **14000.0000** | **3000.0000** | **1274.57 MB** |
| BenchmarkAsync(3000 threads) | Int32[3000] | 2,019.67 ms | 39.676 ms |  65.188 ms | 2,015.46 ms | 279000.0000 | 18000.0000 | 3000.0000 | 1274.57 MB |
