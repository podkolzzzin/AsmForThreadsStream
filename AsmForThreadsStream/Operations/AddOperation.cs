namespace AsmForThreadsStream
{
    class AddOperation : AtomicOperation
    {
        public override ExecutionContext Execute(ExecutionContext context)
        {
            var left = (int)context.Registers[0];
            var right = (int)context.Registers[1];
            context.Registers[0] = left + right;

            return base.Execute(context);
        }
    }
}
