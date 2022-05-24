using System.Collections.Generic;

namespace AsmForThreadsStream
{
    static class OperationExtensions
    {

        public static IEnumerable<IAtomic> Flattern(this IEnumerable<IOperation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation is IAtomic atomic)
                    yield return atomic;
                else
                {
                    foreach (var op in operation.GetOperations())
                    {
                        yield return op;
                    }
                }
            }
        }
    }
}
