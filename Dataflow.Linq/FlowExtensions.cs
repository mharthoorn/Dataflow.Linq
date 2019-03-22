using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Linq
{

    public static class DataFlowExtensions
    {

        public static DataFlow<In, Out> Buffer<In, Out>(this DataFlow<In, Out> source)
        {
            var target = new BufferBlock<Out>();
            return source.Append(target);
        }

        public static DataFlow<In, Target> Transform<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, Target> func, ExecutionDataflowBlockOptions options)
        {
            var target = new TransformBlock<Out, Target>(func, options);
            return source.Append(target);
        }

        public static DataFlow<In, Target> Transform<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, Target> func)
        {
            var target = new TransformBlock<Out, Target>(func);
            return source.Append(target);
        }

        public static DataFlow<In, Target> Transform<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, Task<Target>> func)
        {
            var target = new TransformBlock<Out, Target>(func);
            return source.Append(target);
        } 

        public static DataFlow<In, Target> Transform<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, Target> func, int parallelism)
        {
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = parallelism };
            var target = new TransformBlock<Out, Target>(func);
            return source.Append(target);
        }

        public static DataFlow<In, Target> TransformMany<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, IEnumerable<Target>> func)
        {
            var target = new TransformManyBlock<Out, Target>(func);
            return source.Append(target);
        }

        public static DataFlow<In, Out> Observe<In, Out>(this DataFlow<In, Out> source, Action<Out> action)
        {
            var observer = source.Output.AsObservable();
            observer.Subscribe(action);
            return source;
        }

        public static DataFlow<In, Out> Sniff<In, Out>(this DataFlow<In, Out> source, Action<Out> action)
        {
            var broadcastBlock = new BroadcastBlock<Out>(n => n);

            var actionBlock = new ActionBlock<Out>(action);
            broadcastBlock.LinkTo(actionBlock);

            return source.Append(broadcastBlock);
        }
        public static DataFlow<In, Out> Action<In, Out>(this DataFlow<In, Out> source, Action<Out> action, ExecutionDataflowBlockOptions options)
        {

            var target = new ActionBlock<Out>(action, options);
            return source.Append(target);
        }

        public static DataFlow<In, Out> Action<In, Out>(this DataFlow<In, Out> source, Action<Out> action)
        {
            var target = new ActionBlock<Out>(action);
            return source.Append(target);
        }
     
        public static DataFlow<In, Out> Filter<In, Out>(this DataFlow<In, Out> source, Predicate<Out> predicate, int parallel, int buffer)
        {
            var target = new BufferBlock<Out>();

            var options = new DataflowBlockOptions { BoundedCapacity = buffer, EnsureOrdered = false };
            for(int i = 0; i < parallel; i++)
            {
                var lane = new BufferBlock<Out>(options);
                source.Output.PropagateTo(lane);
                lane.PropagateTo(target, predicate);
                lane.LinkToDiscard(); 
            }

            return new DataFlow<In, Out>(source.Input, target);
        }

        public static DataFlow<In, Out> Filter<In, Out>(this DataFlow<In, Out> source, Predicate<Out> predicate)
        {
            var buffer = new BufferBlock<Out>();

            source.Output.PropagateTo(buffer, predicate);
            source.Output.LinkTo(DataflowBlock.NullTarget<Out>()); // discarded
            return new DataFlow<In, Out>(source.Input, buffer);
        }

    }


}
