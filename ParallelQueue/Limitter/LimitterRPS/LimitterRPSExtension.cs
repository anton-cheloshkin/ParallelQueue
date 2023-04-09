namespace ParallelQueue
{
    public static class LimitterRPSExtension
    {
        public static ParallelQueueExecutor WithLimitterRPS(this ParallelQueueExecutor executor, int rps)
        {
            executor.Queue.WithLimitterRPS(rps);
            return executor;
        }
        static ParallelQueue WithLimitterRPS(this ParallelQueue queue, int rps)
        {
            queue.Limitter?.Dispose();
            queue.Limitter = new LimitterRPS(queue, rps);
            return queue;
        }
    }
}
