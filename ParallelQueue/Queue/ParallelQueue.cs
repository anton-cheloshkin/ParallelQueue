using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelQueue
{
    partial class ParallelQueue
    {
        long threads = 0;
        readonly ConcurrentQueue<Func<Task>> Queue;
        public int Count => Queue.Count;
        public ParallelQueue()
        {
            Queue = new ConcurrentQueue<Func<Task>>();
        }
        public void Enqueue(Func<Task> fn)
        {
            Queue.Enqueue(fn);
        }
        SemaphoreSlim Sema = new SemaphoreSlim(Environment.ProcessorCount);
        public async Task EnqueueAsync(Func<Task> fn)
        {
            await Sema.WaitAsync();
            Queue.Enqueue(fn);
            Sema.Release();
        }
        public Func<Task> Dequeue()
        {
            Func<Task> fn;
            Queue.TryDequeue(out fn);
            return fn;
        }
        public void BeforeRun()
        {
            Interlocked.Increment(ref threads);
        }
        public void AfterRun()
        {
            Interlocked.Decrement(ref threads);
        }
        public bool AfterRunBool()
        {
            var th = Interlocked.Decrement(ref threads);

            if (th == 0)
            {
                FireDone();
                return false;
            }
            return true;
        }
        public void Done()
        {
            var th = Interlocked.Read(ref threads);
            if (th == 0) FireDone();
        }
        List<Action> OnDoneListeners = new List<Action>();
        void FireDone()
        {
            Action[] listActions;
            lock (OnDoneListeners)
            {
                listActions = OnDoneListeners.ToArray();
                OnDoneListeners = new List<Action>();
            }
            foreach (var action in listActions) action();
        }
        public void OnDone(Action fn)
        {
            lock (OnDoneListeners)
                if (Queue.Count > 0) OnDoneListeners.Add(fn);
                else fn();

        }
        public Task OnDoneAsync()
        {
            var src = new TaskCompletionSource<Task>();
            var task = src.Task;

            lock (OnDoneListeners)
                if (Queue.Count > 0) OnDoneListeners.Add(() => src.SetResult(Task.CompletedTask));
                else src.SetResult(Task.CompletedTask);

            return task;
        }
        public Task OnDoneAsync(Action fn)
        {
            var src = new TaskCompletionSource<Task>();
            var task = src.Task;

            lock (OnDoneListeners)
                if (Queue.Count > 0) OnDoneListeners.Add(() =>
                {
                    fn();
                    src.SetResult(Task.CompletedTask);
                });
                else
                {
                    fn();
                    src.SetResult(Task.CompletedTask);
                }

            return task;
        }
    }
}
