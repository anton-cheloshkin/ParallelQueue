using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelQueue
{
    internal class LimitterRPS : ILimitter
    {
        readonly ConcurrentQueue<Func<Task>> Queue;
        public int Count => Queue.Count;
        ParallelQueue TaskBag;
        long isRunning = 0;
        public int LimitInt { get; set; }
        public LimitterRPS(ParallelQueue executor, int rPS = 8)
        {
            Queue = new ConcurrentQueue<Func<Task>>();
            TaskBag = executor;
            LimitInt = rPS;
        }
        public void Enqueue(Func<Task> fn)
        {
            if (isDisposing)
                throw new Exception("disposing");

            Queue.Enqueue(fn);
            Task.Run(Execute);
        }
        Func<Task> Dequeue()
        {
            Queue.TryDequeue(out Func<Task> fn);
            return fn;
        }
        async ValueTask Execute()
        {
            var val = Interlocked.Increment(ref isRunning);
            if (val > 1)
            {
                Interlocked.Decrement(ref isRunning);
                return;
            }

            var counter = LimitInt;
            while (counter-- > 0)
            {
                var fn = Dequeue();
                if (fn != null)
                {
                    TaskBag.Enqueue(fn);
                }
            }

            await Task.Delay(1000);
            Interlocked.Decrement(ref isRunning);

            if (Count == 0 && isDisposing)
            {
                TaskBag = null;
                return;
            }

            await Task.Run(Execute);
        }
        bool isDisposing = false;
        public void Dispose()
        {
            isDisposing = true;
        }
    }
}
