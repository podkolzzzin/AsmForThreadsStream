using System.Linq;

namespace AsmForThreadsStream
{
    class ExecutionThread
    {
        private readonly IAtomicOperation[] program;

        private ExecutionContext executionContext = new ExecutionContext(0, new object[2]);

        public ExecutionThread(params IOperation[] program)
        {
            this.program = program.Flattern().ToArray();
        }

        public bool ExecuteNext()
        {
            var current = program[executionContext.Current];
            executionContext = current.Execute(executionContext);
            return executionContext.Current < program.Length;
        }

        public void Execute()
        {
            while (ExecuteNext())
                ;
        }
    }
}
