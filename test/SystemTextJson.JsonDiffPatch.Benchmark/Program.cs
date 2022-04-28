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
                StatisticColumn.P80,
                StatisticColumn.P95);
            config.AddColumnProvider(DefaultColumnProviders.Metrics);
            config.AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)));
            config.AddValidator(JitOptimizationsValidator.FailOnError);
            config.AddLogger(new ConsoleLogger(true));
            config.AddExporter(MarkdownExporter.GitHub);

            BenchmarkRunner.Run<OptionsBenchmarks>(config);
            BenchmarkRunner.Run<DeepEqualsBenchmarks>(config);
            BenchmarkRunner.Run<DemoObjectBenchmarks>(config);
            BenchmarkRunner.Run<LargeObjectBenchmarks>(config);
        }
    }
}
