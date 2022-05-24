using System.Collections.Generic;

namespace AsmForThreadsStream
{
    abstract class AtomicOperation : IAtomic, IOperation
    {
        public IEnumerable<IAtomic> GetOperations()
        {
            yield return this;
        }

        public virtual ExecutionContext Execute(ExecutionContext context)
        {
            return new ExecutionContext(context.Current + 1, context.Registers);
        }
    }
}
