namespace AsmForThreadsStream
{
    class GotoOperation : AtomicOperation
    {
        private readonly int? _relativeAddress;

        public GotoOperation(int relativeAddress)
        {
            _relativeAddress = relativeAddress;
        }

        public GotoOperation()
        {
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            int gotoAddress;
            if (_relativeAddress != null)
                gotoAddress = context.Current + _relativeAddress.Value;
            else
                gotoAddress = (int)context.Registers[0];

            return new ExecutionContext(gotoAddress, context.Registers);
        }
    }
}
