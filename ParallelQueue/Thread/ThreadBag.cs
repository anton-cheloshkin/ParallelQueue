using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelQueue
{
    class ThreadBag
    {
        ParallelQueue Queue;
        List<ThreadExecutor> Threads;
        public ThreadBag(ParallelQueue queue)
        {
            Queue = queue;
            Threads = new List<ThreadExecutor>(0);
        }
        public void Resize(int size)
        {
            lock (Threads)
            {
                var oldThreads = Threads;
                Threads = new List<ThreadExecutor>(size);

                foreach (var thread in Threads.Skip(size))
                {
                    thread.Dispose();
                }

                var addThreads = Enumerable
                    .Range(0, Math.Max(0, size - oldThreads.Count))
                    .Select((i) => new ThreadExecutor(Queue));

                var newThreads = oldThreads
                    .Take(size)
                    .Union(addThreads);

                Threads.AddRange(newThreads);
            }
        }
        public void Execute()
        {
            var executor = Threads.OrderBy(r => r.IsRunning).FirstOrDefault();
            if (executor == null) return;

            Task.Run(async () => await executor.Execute());
        }
    }
}
