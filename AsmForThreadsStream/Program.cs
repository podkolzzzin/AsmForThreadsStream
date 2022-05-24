using System.Text;
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
}
