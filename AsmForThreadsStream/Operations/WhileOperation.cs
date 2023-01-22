using System.Collections.Generic;
using System.Linq;

namespace AsmForThreadsStream
{
    class WhileOperation : IOperation
    {
        private readonly IOperation[] _body;
        private readonly IOperation[] _condition;

        public WhileOperation(IOperation[] condition, params IOperation[] body)
        {
            _condition = condition;
            _body = body;
        }

        public IEnumerable<IAtomicOperation> GetOperations()
        {
            var ops = new IfOperation(_condition, _body, new IOperation[0]).GetOperations();
            var gotoP = -ops.Count();

            ops = new IfOperation(_condition, _body.Concat(new[] { new GotoOperation(gotoP) }).ToArray(), new IOperation[0]).GetOperations();
            foreach (var atomic in ops)
                yield return atomic;
        }
    }
}
