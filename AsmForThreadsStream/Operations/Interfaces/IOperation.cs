using System.Collections.Generic;

namespace AsmForThreadsStream
{
    interface IOperation
    {
        IEnumerable<IAtomicOperation> GetOperations();
    }
}
