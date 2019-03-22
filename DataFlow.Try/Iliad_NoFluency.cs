using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks.Dataflow;

namespace DataFlowTest
{
    public partial class Iliad
    {
        public static (ITargetBlock<string> input, IDataflowBlock output) DataflowWithoutFluency()
        { 

            var downloadString = new TransformBlock<string, string>(async uri =>
            {
                return await new HttpClient().GetStringAsync(uri);
            });

            var createWordList = new TransformBlock<string, string[]>(text =>
            {
                char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
                text = new string(tokens);

                return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            });

            var filterWordList = new TransformBlock<string[], string[]>(words =>
            {
                return words
                   .Where(word => word.Length > 3)
                   .Distinct()
                   .ToArray();
            });

            var findReversedWords = new TransformManyBlock<string[], string>(words =>
            {
                var wordsSet = new HashSet<string>(words);

                return from word in words.AsParallel()
                       let reverse = new string(word.Reverse().ToArray())
                       where word != reverse && wordsSet.Contains(reverse)
                       select word;
            });

            var printReversedWords = new ActionBlock<string>(reversedWord =>
            {
                Console.WriteLine("Found reversed words {0}/{1}",
                   reversedWord, new string(reversedWord.Reverse().ToArray()));
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            downloadString.LinkTo(createWordList, linkOptions);
            createWordList.LinkTo(filterWordList, linkOptions);
            filterWordList.LinkTo(findReversedWords, linkOptions);
            findReversedWords.LinkTo(printReversedWords, linkOptions);

            return (downloadString, printReversedWords);
        }

    }
}
