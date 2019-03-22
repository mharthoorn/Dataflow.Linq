using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Linq
{
    public static class DataFlow  
    {
        public static DataFlow<T, T> From<T>()
        {
            var buffer = new BufferBlock<T>();
            return new DataFlow<T, T>(buffer, buffer);
        }
    }
     
    public struct DataFlow<In, Out> : IFlow<In> 
    {
        public ITargetBlock<In> Input;
        public ISourceBlock<Out> Output;
        public IDataflowBlock Tail;

        public DataFlow(ITargetBlock<In> input, ISourceBlock<Out> output) : this(input, output, output)
        { }

        public DataFlow(ITargetBlock<In> input, ISourceBlock<Out> output, IDataflowBlock tail)
        {
            this.Input = input;
            this.Output = output;
            this.Tail = tail;
        }

        public DataFlow<In, T> Append<T>(IPropagatorBlock<Out, T> target)
        {
            Output.PropagateTo(target);

            return new DataFlow<In, T>(Input, target, target);
        }

        public DataFlow<In, Out> Append(ITargetBlock<Out> target)
        {
            Output.PropagateTo(target);
            return new DataFlow<In, Out>(this.Input, this.Output, target);
        }

        public DataFlow<In, T> Append<T>(DataFlow<Out, T> target)
        {
            Output.PropagateTo(target.Input);
            return new DataFlow<In, T>(this.Input, target.Output, target.Tail);
        }

        public void Complete()
        {
            Input.Complete();
        }

        public Task Completion => Tail.Completion;

        public void Fault(Exception exception)
        {
            Input.Fault(exception);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, In messageValue, ISourceBlock<In> source, bool consumeToAccept)
        {
            return this.Input.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

    }

}
