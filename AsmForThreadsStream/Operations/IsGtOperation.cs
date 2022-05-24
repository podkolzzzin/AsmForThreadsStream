namespace AsmForThreadsStream
{
    class IsGtOperation : AtomicOperation
    {
        public override ExecutionContext Execute(ExecutionContext context)
        {
            var x = (int)context.Registers[0];
            var y = (int)context.Registers[1];
            context.Registers[0] = x > y;
            return base.Execute(context);
        }
    }
}
