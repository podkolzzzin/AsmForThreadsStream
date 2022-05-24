using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AsmForThreadsStream
{
    class MultiThreadPlanner
    {
        private readonly List<ExecutionThread> _threads;
        private readonly List<Thread> _nativeThreads;
        private readonly ThreadLocal<Random> _random;

        public MultiThreadPlanner(int count, params ExecutionThread[] threads)
        {
            _random = new ThreadLocal<Random>(() => new Random());
            _nativeThreads = Enumerable.Repeat(0, count).Select(x => new Thread(ExecuteInternal))
                .ToList();
            _threads = threads.ToList();
        }

        private void ExecuteInternal()
        {
            ExecutionThread current;
            while ((current = GetRandomExecutionThread()) != null)
            {
                Console.WriteLine("ManagedThreadId = " + Thread.CurrentThread.ManagedThreadId);
                var quant = _random.Value.Next(0, 10);
                for (int i = 0; i < quant; i++)
                {
                    if (!current.ExecuteNext())
                    {
                        return;
                    }
                }

                lock (_threads)
                    _threads.Add(current);
            }
        }

        private ExecutionThread GetRandomExecutionThread()
        {
            lock (_threads)
            {
                if (_threads.Count == 0)
                    return null;
                var v = _random.Value.Next(0, _threads.Count);
                var thread = _threads[v];
                _threads.RemoveAt(v);
                return thread;
            }
        }

        public void Execute()
        {
            foreach (var item in _nativeThreads)
                item.Start();
            foreach (var item in _nativeThreads)
                item.Join();
        }

        private void ExecuteOperations(Random random, ExecutionThread item)
        {
            var x = random.Next(1, 20);
            for (int i = 0; i < x; i++)
            {
                if (!item.ExecuteNext())
                {
                    _threads.Remove(item);
                    break;
                }
            }
        }
    }
}
