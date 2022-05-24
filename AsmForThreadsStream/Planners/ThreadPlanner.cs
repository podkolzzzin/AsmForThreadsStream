using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmForThreadsStream
{
    class ThreadPlanner
    {
        private readonly List<ExecutionThread> _threads;

        public ThreadPlanner(params ExecutionThread[] threads)
        {
            _threads = threads.ToList();
        }

        public void Execute()
        {
            var random = new Random();
            while (_threads.Count > 0)
            {
                foreach (var item in _threads.ToArray())
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
    }
}
