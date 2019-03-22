using System.Threading.Tasks;

namespace DataFlowTest
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            await NumericsLinqFlow.Test();
        }
    }
}
