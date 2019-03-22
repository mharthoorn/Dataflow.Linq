using System;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Linq
{
    public static class DataflowLinkExtensions
    {
        private static DataflowLinkOptions options = new DataflowLinkOptions { PropagateCompletion = true };

        public static IDisposable PropagateTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            return source.LinkTo(target, options);
        }

        public static IDisposable PropagateTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target, Predicate<T> predicate)
        {
            return source.LinkTo(target, options, predicate);
        }

        public static IDisposable LinkToDiscard<T>(this ISourceBlock<T> source)
        {
            return source.LinkTo(DataflowBlock.NullTarget<T>()); // discard
        }


    }
}
