using System.Collections.Generic;

namespace AsmForThreadsStream
{
    static class OperationExtensions
    {

        public static IEnumerable<IAtomicOperation> Flattern(this IEnumerable<IOperation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation is IAtomicOperation atomic)
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
