namespace AsmForThreadsStream
{
    class WriteOperation : AtomicOperation
    {
        private readonly string _address;

        public WriteOperation(string address)
        {
            _address = address;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            Memory.Write(context.Registers[0], _address);
            return base.Execute(context);
        }
    }
}
