using System;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ManualConfig();
            config.AddColumn(
                TargetMethodColumn.Method,
                StatisticColumn.Mean,
                StatisticColumn.Min,
                StatisticColumn.Max,
                StatisticColumn.P95,
                StatisticColumn.P80);
            config.AddColumnProvider(DefaultColumnProviders.Metrics);
            config.AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)));
            config.AddValidator(JitOptimizationsValidator.FailOnError);
            config.AddLogger(new ConsoleLogger(true));
            config.AddExporter(MarkdownExporter.GitHub);

            BenchmarkRunner.Run<DemoObjectBenchmarks>(config);
            BenchmarkRunner.Run<LargeObjectBenchmarks>(config);
            Console.ReadLine();
        }
    }
}
