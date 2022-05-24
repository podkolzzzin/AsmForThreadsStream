namespace AsmForThreadsStream
{
    class PutConstantToRigister : AtomicOperation
    {
        private readonly object _constant;
        private readonly int _register;

        public PutConstantToRigister(object constant, int register)
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
