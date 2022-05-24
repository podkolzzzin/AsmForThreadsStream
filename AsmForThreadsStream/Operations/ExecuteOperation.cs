using System;

namespace AsmForThreadsStream
{
    class ExecuteOperation : AtomicOperation
    {
        private readonly Action<ExecutionContext> _action;

        public ExecuteOperation(Action<ExecutionContext> action)
        {
            _action = action;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            _action(context);
            return base.Execute(context);
        }
    }
}
