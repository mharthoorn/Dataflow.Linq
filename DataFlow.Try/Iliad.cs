using System.Threading.Tasks.Dataflow;
using Dataflow.Linq;
using System.Threading.Tasks;

namespace DataFlowTest
{

    public partial class Iliad
    {

        public async Task Test()
        {
            var flow = GetFlow();
            flow.Post("http://www.gutenberg.org/files/6130/6130-0.txt");
            await flow.FinishAsync();

            var (input, output) = DataflowWithoutFluency();
            input.Post("http://www.gutenberg.org/files/6130/6130-0.txt");
            input.Complete();
            output.Completion.Wait();

            
        }
    }
}
