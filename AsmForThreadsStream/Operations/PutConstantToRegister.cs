namespace AsmForThreadsStream
{
    class PutConstantToRegister : AtomicOperation
    {
        private readonly object _constant;
        private readonly int _register;

        public PutConstantToRegister(object constant, int register)
        {
            this._register = register;
            this._constant = constant;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            context.Registers[_register] = _constant;
            return base.Execute(context);
        }
    }
}
