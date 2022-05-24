namespace AsmForThreadsStream
{
    class ExecutionContext
    {
        public int Current { get; }
        public object[] Registers { get; }


        public ExecutionContext(int current, object[] registers)
        {
            Current = current;
            Registers = registers;
        }
    }
}
