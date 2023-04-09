using System;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new ParallelQueueTest().Test();
            await new ParallelQueueTest().TestAsync();
            //await new ParallelQueueTest().TestLimitterAsync();
            Console.WriteLine("Hello World!");
        }
    }
}
