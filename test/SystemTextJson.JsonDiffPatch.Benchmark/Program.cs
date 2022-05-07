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
                StatisticColumn.Median,
                StatisticColumn.P80,
                StatisticColumn.P95,
                StatisticColumn.Min,
                StatisticColumn.Max);
            config.AddColumnProvider(DefaultColumnProviders.Params);
            config.AddColumnProvider(DefaultColumnProviders.Metrics);
            config.AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)));
            config.AddValidator(JitOptimizationsValidator.FailOnError);
            config.AddLogger(new ConsoleLogger(true));
            config.AddExporter(MarkdownExporter.GitHub);

            BenchmarkSwitcher.FromAssembly(typeof(BenchmarkHelper).Assembly).Run(args, config);
        }
    }
}
