using System.Threading.Tasks.Dataflow;
using Dataflow.Linq;
using System.Threading.Tasks;

namespace DataFlowTest
{

    public partial class Iliad
    {
        public const string GUTENBERG_URL = "http://www.gutenberg.org/files/6130/6130-0.txt";

        public static async Task Test()
        {
            var flow = GetFlow();
            flow.Post(GUTENBERG_URL);
            await flow.FinishAndWait();

            var (input, output) = DataflowWithoutFluency();
            input.Post(GUTENBERG_URL);
            input.Complete();
            output.Completion.Wait();

            
        }
    }
}
