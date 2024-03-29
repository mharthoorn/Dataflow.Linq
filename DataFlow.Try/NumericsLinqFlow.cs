﻿using System;
using System.Reactive.Linq;
using System.Linq;
using Dataflow.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

namespace DataFlowTest
{
    public static class NumericsLinqFlow
    {
        public static bool Prime(int n)
        {
            for (int i = 2; i < n; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }


        public static IFlow<int> GetLinqFlow()
        {
            return
                (
                    from a in DataFlow.From<int>()
                    where Prime(a)
                    select a
                )
                .Action(Console.WriteLine);
        }

        public static IFlow<int> GetFlow()
        {
            var cache = new List<int>();
            return
                DataFlow
                .From<int>()
                .Filter(i => Prime(i), parallel: 10, buffer: 10)
                .Action(Console.WriteLine);
        }

        public static async Task TestLinq()
        {
            var flow = GetLinqFlow();
            Enumerable.Range(1, 30).Post(flow);
            await flow.FinishAndWait();
        }

        public static async Task Test()
        {
            var flow = GetFlow();
            flow.PostAll(Enumerable.Range(10_000_000, 10_000_000));
            await flow.FinishAndWait();
        }

    }
}
