using System.Collections.Generic;
using System.Linq;

namespace AsmForThreadsStream
{
    class IfOperation : IOperation
    {
        private class IfAtomic : AtomicOperation
        {
            public override ExecutionContext Execute(ExecutionContext context)
            {
                var val = (bool)context.Registers[0];
                return new ExecutionContext(context.Current + (val ? 2 : 1), context.Registers);
            }
        }

        private IOperation[] _condition, _ifTrueClause, _ifFalseClause;

        public IfOperation(IOperation[] condition, IOperation[] ifTrueClause, IOperation[] ifFalseClause)
        {
            _condition = condition;
            _ifTrueClause = ifTrueClause;
            _ifFalseClause = ifFalseClause;
        }

        public IEnumerable<IAtomicOperation> GetOperations()
        {
            var conditionOps = _condition.Flattern().ToArray();
            foreach (var item in conditionOps)
                yield return item;

            yield return new IfAtomic();
            var trueOps = _ifTrueClause.Flattern().ToArray();
            var falseOps = _ifFalseClause.Flattern().ToArray();

            yield return new GotoOperation(trueOps.Length + 1);

            foreach (var op in trueOps)
            {
                yield return op;
            }
            yield return new GotoOperation(falseOps.Length + 1);

            foreach (var op in falseOps)
            {
                yield return op;
            }
        }
    }
}
