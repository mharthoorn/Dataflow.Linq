using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflow.Linq
{
    public interface IFlowProcessor<A, B>
    {
        Task<B> Process(A input);
    }

    public static class IFlowProcessorExtensions
    {
        public static DataFlow<In, Target> Transform<In, Out, Target>(this DataFlow<In, Out> source, IFlowProcessor<Out, Target> processor)
        {
            Func<Out, Task<Target>> func = processor.Process;
            return source.Transform(processor.Process);
        }
 
        public static DataFlow<In, Target> TransformMany<In, Out, Target>(this DataFlow<In, Out> source, IFlowProcessor<Out, IEnumerable<Target>> processor)
        {
            return source.TransformMany(processor.Process); 
        }

    }
}
