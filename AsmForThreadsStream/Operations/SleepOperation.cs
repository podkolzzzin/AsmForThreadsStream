using System.Diagnostics;

namespace AsmForThreadsStream
{
    sealed class SleepOperation : AtomicOperation
    {
        private readonly int _durationMs;
        private Stopwatch stopwatch;

        public SleepOperation(int durationMs)
        {
            _durationMs = durationMs;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            if (stopwatch== null)
                stopwatch = Stopwatch.StartNew();
            if (stopwatch.ElapsedMilliseconds > _durationMs)
            {
                stopwatch = null;
                return base.Execute(context);
            }
            return context;
        }
    }
}
