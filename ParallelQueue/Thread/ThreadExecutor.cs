using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelQueue
{
    partial class ThreadExecutor : IDisposable
    {
        ParallelQueue Bag;
        long isRunning = 0;
        bool isDisposing = false;
        public long IsRunning => Interlocked.Read(ref isRunning);
        public ThreadExecutor(ParallelQueue bag)
        {
            Bag = bag;
        }
        public async ValueTask Execute()
        {

            var val = Interlocked.Increment(ref isRunning);

            if (val > 1)
            {
                Interlocked.Decrement(ref isRunning);
                return;
            }

            if (isDisposing)
            {
                Interlocked.Decrement(ref isRunning);
                Bag.Done();
                Bag = null;
                return;
            }

            var task = Bag.Dequeue();
            if (task == null)
            {
                Interlocked.Decrement(ref isRunning);
                Bag.Done();
                return;
            }

            Bag.BeforeRun();
            await task();
            Interlocked.Decrement(ref isRunning);

            Bag.AfterRun();

            /*
            if (!Bag.AfterRunBool())
            {
                Interlocked.Decrement(ref isRunning);
                return;
            }
            */

            await Task.Run(Execute);
        }
        public void Dispose()
        {
            isDisposing = true;
        }
    }

}