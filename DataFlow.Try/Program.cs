using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Dataflow.Linq;
using System.Collections.Generic;

namespace DataFlowTest
{
    public static class TransformManyTest
    {
        public static IEnumerable<int> List(int i)
        {
            for(int j = 1; j <= i; j++)
            {
                yield return j;
            }
        }

        public static async Task Run()
        {
            var m = new TransformManyBlock<int, int>(List);
            var a = new ActionBlock<int>(Console.WriteLine);
            m.PropagateTo(a);

            m.Post(8);
            m.Complete();
            await m.Completion;
        }
    }

    class Program
    {
       
        static async Task Main(string[] args)
        {

            await TransformManyTest.Run();

            Console.WriteLine("Press any key.");
            Console.ReadKey();
        }
    }
}
