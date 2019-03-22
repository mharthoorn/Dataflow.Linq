using System;
using System.Linq;
using Dataflow.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace DataFlowTest
{
    public partial class Iliad 
    {
        public static IFlow<string> GetFlow()
        {
            return DataFlow
                .From<string>()

                .Transform(async uri => await new HttpClient().GetStringAsync(uri))

                .Transform(text =>
                {
                    char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
                    return new string(tokens).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                })

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
