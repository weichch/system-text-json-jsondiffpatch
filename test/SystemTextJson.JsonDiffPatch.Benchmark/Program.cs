using System;
using BenchmarkDotNet.Running;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SimpleDiffBenchmark>();
            Console.ReadLine();
        }
    }
}
