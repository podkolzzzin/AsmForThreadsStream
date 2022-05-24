using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsmForThreadsStream
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var redLamp = new Lamp();
            var greenLamp = new Lamp();

            var t1 = new ExecutionThread(
                new PutConstantToRigister(true, 0),
                new WhileOperation(new IOperation[]
                    {
                        new ExecuteOperation(ctx => redLamp.TurnOn()),
                        new SleepOperation(200),
                        new ExecuteOperation(ctx => redLamp.TurnOff()),
                        new SleepOperation(1000)
                    })
                ); // x = 0; while (x < 10) cw("Hello");


            var t2 = new ExecutionThread(
                new PutConstantToRigister(true, 0),
                new WhileOperation(new IOperation[]
                    {
                        new ExecuteOperation(ctx => greenLamp.TurnOn()),
                        new SleepOperation(500),
                        new ExecuteOperation(ctx => greenLamp.TurnOff()),
                        new SleepOperation(500)
                    })
                );


            new ThreadPlanner(t1, t2).Execute();
        }
    }

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

    class ThreadPlanner
    {
        private readonly List<ExecutionThread> _threads;

        public ThreadPlanner(params ExecutionThread[] threads)
        {
            _threads = threads.ToList();
        }

        public void Execute()
        {
            var random = new Random();
            while (_threads.Count > 0)
            {
                foreach (var item in _threads.ToArray())
                {
                    var x = random.Next(1, 20);
                    for (int i = 0; i < x; i++)
                    {
                        if (!item.ExecuteNext())
                        {
                            _threads.Remove(item);
                            break;
                        }
                    }
                }
            }
        }
    }



    class MultiThreadPlanner
    {
        private readonly List<ExecutionThread> _threads;
        private readonly List<Thread> _nativeThreads;
        private readonly ThreadLocal<Random> _random;

        public MultiThreadPlanner(int count, params ExecutionThread[] threads)
        {
            _random = new ThreadLocal<Random>(() => new Random());
            _nativeThreads = Enumerable.Repeat(0, count).Select(x => new Thread(ExecuteInternal))
                .ToList();
            _threads = threads.ToList();
        }

        private void ExecuteInternal()
        {
            ExecutionThread current;
            while ((current = GetRandomExecutionThread()) != null)
            {
                Console.WriteLine("ManagedThreadId = " + Thread.CurrentThread.ManagedThreadId);
                var quant = _random.Value.Next(0, 10);
                for (int i = 0; i < quant; i++)
                {
                    if (!current.ExecuteNext())
                    {
                        return;
                    }
                }

                lock (_threads)
                    _threads.Add(current);
            }
        }

        private ExecutionThread GetRandomExecutionThread()
        {
            lock (_threads)
            {
                if (_threads.Count == 0)
                    return null;
                var v = _random.Value.Next(0, _threads.Count);
                var thread = _threads[v];
                _threads.RemoveAt(v);
                return thread;
            }
        }

        public void Execute()
        {
            foreach (var item in _nativeThreads)
                item.Start();
            foreach (var item in _nativeThreads)
                item.Join();
        }

        private void ExecuteOperations(Random random, ExecutionThread item)
        {
            var x = random.Next(1, 20);
            for (int i = 0; i < x; i++)
            {
                if (!item.ExecuteNext())
                {
                    _threads.Remove(item);
                    break;
                }
            }
        }
    }

    class ExecutionThread
    {
        private readonly IAtomic[] program;

        private ExecutionContext executionContext = new ExecutionContext(0, new object[2]);

        public ExecutionThread(params IOperation[] program)
        {
            this.program = program.Flattern().ToArray();
        }

        public bool ExecuteNext()
        {
            var current = program[executionContext.Current];
            executionContext = current.Execute(executionContext);
            return executionContext.Current < program.Length;
        }

        public void Execute()
        {
            while (ExecuteNext())
                ;
        }
    }

    interface IAtomic
    {
        ExecutionContext Execute(ExecutionContext context);
    }

    interface IOperation
    {
        IEnumerable<IAtomic> GetOperations();
    }

    abstract class AtomicOperation : IAtomic, IOperation
    {
        public IEnumerable<IAtomic> GetOperations()
        {
            yield return this;
        }

        public virtual ExecutionContext Execute(ExecutionContext context)
        {
            return new ExecutionContext(context.Current + 1, context.Registers);
        }
    }

    static class Memory
    {
        private static readonly Dictionary<string, object> _ram = new Dictionary<string, object>();

        public static T Read<T>(string address)
        {
            return (T)_ram[address];
        }

        public static void Write<T>(T obj, string address)
        {
            _ram[address] = obj;
        }
    }
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

    sealed class SleepOperation : AtomicOperation
    {
        private readonly int _durationMs;
        private Stopwatch stopwatch;

        public SleepOperation(int durationMs)
        {
            _durationMs = durationMs;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            if (stopwatch== null)
                stopwatch = Stopwatch.StartNew();
            if (stopwatch.ElapsedMilliseconds > _durationMs)
            {
                stopwatch = null;
                return base.Execute(context);
            }
            return context;
        }
    }

    class ExecuteOperation : AtomicOperation
    {
        private readonly Action<ExecutionContext> _action;

        public ExecuteOperation(Action<ExecutionContext> action)
        {
            _action = action;
        }

        public override ExecutionContext Execute(ExecutionContext context)
        {
            _action(context);
            return base.Execute(context);
        }
    }

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

    class IsLtOperation : AtomicOperation
    {
        public override ExecutionContext Execute(ExecutionContext context)
        {
            var x = (int)context.Registers[0];
            var y = (int)context.Registers[1];
            context.Registers[0] = x < y;
            return base.Execute(context);
        }
    }

    static class OperationExtensions
    {

        public static IEnumerable<IAtomic> Flattern(this IEnumerable<IOperation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation is IAtomic atomic)
                    yield return atomic;
                else
                {
                    foreach (var op in operation.GetOperations())
                    {
                        yield return op;
                    }
                }
            }
        }
    }

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

        public IEnumerable<IAtomic> GetOperations()
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

    class IncrementOperation : IOperation
    {
        private readonly string _address;

        public IncrementOperation(string address)
        {
            _address = address;
        }

        public IEnumerable<IAtomic> GetOperations()
        {
            yield return new ReadOperation(_address, 0);
            yield return new PutConstantToRigister(1, 1);
            yield return new AddOperation();
            yield return new WriteOperation(_address);
        }
    }

    class WhileOperation : IOperation
    {
        private readonly IOperation[] _body;
        private readonly IOperation[] _condition;

        public WhileOperation(IOperation[] condition, params IOperation[] body)
        {
            _condition = condition;
            _body = body;
        }

        public IEnumerable<IAtomic> GetOperations()
        {
            var ops = new IfOperation(_condition, _body, new IOperation[0]).GetOperations();
            var gotoP = -ops.Count();

            ops = new IfOperation(_condition, _body.Concat(new[] { new GotoOperation(gotoP) }).ToArray(), new IOperation[0]).GetOperations();
            foreach (var atomic in ops)
                yield return atomic;
        }
    }
}
