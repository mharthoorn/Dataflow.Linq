using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Linq
{
    public interface IFlow<T> : IDataflowBlock, ITargetBlock<T>
    {

    }

    public static class IFlowExtensions
    {
        public static async ValueTask FinishAsync<T>(this IFlow<T> flow)
        {
            flow.Complete();
            await flow.Completion;
        }

        public static void Post<T>(this IEnumerable<T> range, IFlow<T> flow)
        {
            foreach (var item in range)
            {
                flow.Post(item);
            }
        }

        public static void PostAll<T>(this IFlow<T> flow, IEnumerable<T> range)
        { 
            foreach (var item in range)
            {
                flow.Post(item);
            }
        }
    }



}
