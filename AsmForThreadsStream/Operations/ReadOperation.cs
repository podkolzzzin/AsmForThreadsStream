namespace AsmForThreadsStream
{
    sealed class ReadOperation : AtomicOperation
    {
        private readonly string _address;
        private readonly int _register;

        public ReadOperation(string address, int register)
        {
            _address = address;
            _register = register;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            context.Registers[_register] = Memory.Read<object>(_address);
            return base.Execute(context);
        }
    }
}
