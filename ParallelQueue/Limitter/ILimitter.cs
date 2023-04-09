using System;
using System.Threading.Tasks;

namespace ParallelQueue
{
    internal interface ILimitter : IDisposable
    {
        int Count { get; }
        void Enqueue(Func<Task> fn);
    }
}
