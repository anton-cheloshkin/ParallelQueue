using System;
using System.Threading.Tasks;

namespace ParallelQueue
{
    public partial class ParallelQueueExecutor
    {
        internal ParallelQueue Queue;
        ThreadBag ThreadPool;
        int MaxThreads = Environment.ProcessorCount - 1;
        public int Count => Queue.Count;
        public int DegreeOfParallelism
        {
            get => Math.Min(Math.Max(1, MaxThreads), 512);
            set
            {
                MaxThreads = value;
                ThreadPool.Resize(DegreeOfParallelism);
            }
        }
        public ParallelQueueExecutor()
        {
            Queue = new ParallelQueue();
            ThreadPool = new ThreadBag(Queue);
            ThreadPool.Resize(DegreeOfParallelism);
        }
        public void Enqueue(Func<Task> fn)
        {
            Queue.Enqueue(fn);
            ThreadPool.Execute();
        }
        public async Task EnqueueAsync(Func<Task> fn)
        {
            await Queue.EnqueueAsync(fn);
            ThreadPool.Execute();
        }
        public void OnDone(Action fn)
        {
            Queue.OnDone(fn);
        }
        public Task OnDoneAsync()
        {
            return Queue.OnDoneAsync();
        }
        public Task OnDoneAsync(Action fn)
        {
            return Queue.OnDoneAsync(fn);
        }
    }
}
