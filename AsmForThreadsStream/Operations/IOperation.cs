using System.Collections.Generic;

namespace AsmForThreadsStream
{
    interface IOperation
    {
        IEnumerable<IAtomic> GetOperations();
    }
}
