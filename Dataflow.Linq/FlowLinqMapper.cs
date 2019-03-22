using System;
using System.Collections.Generic;

namespace Dataflow.Linq
{
    public static class DataFlowLinqMapExtensions
    {
        public static DataFlow<In, Out> Where<In, Out>(this DataFlow<In, Out> source, Predicate<Out> predicate)
            => source.Filter(predicate);

        public static DataFlow<In, Target> Select<In, Out, Target>(this DataFlow<In, Out> source, Func<Out, Target> func)
            => source.Transform(func);
    }

}
