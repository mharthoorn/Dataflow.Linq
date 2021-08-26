using System;
using System.Linq;
using Dataflow.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataFlowTest
{
   
    public class HttpStringContentGetter : IFlowProcessor<string, string>
    {
        public async Task<string> Process(string uri)
        {
            return await new HttpClient().GetStringAsync(uri);
        }
    }

    public class WordSplitter : IFlowProcessor<string, string[]>
    {
        public async Task<string[]> Process(string text)
        {
            char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
            var result = new string(tokens).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return await Task.FromResult(result);
        }
    }

    public partial class Iliad
    {
        public static IFlow<string> GetProcessorsFlow()
        {
            return DataFlow
                .From<string>()

                .Transform(new HttpStringContentGetter())

                .Transform(new WordSplitter())

                .Transform(words => words.Where(word => word.Length > 3).Distinct().ToArray())

                .TransformMany(words =>
                {
                    var wordSet = new HashSet<string>(words);
                    return from word in words.AsParallel()
                           let reverse = new string(word.Reverse().ToArray())
                           where word != reverse && wordSet.Contains(reverse)
                           select word;
                })
                .Action(
                    r => Console.WriteLine($"Found reversed word: {r}/{ new string(r.Reverse().ToArray()) }"));
        }
    }
}
