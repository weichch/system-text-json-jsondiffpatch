using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DemoObjectBenchmarks : DiffPatchBenchmarks
    {
        [Params(@"Examples\demo_left.json")]
        public override string BeforeFile { get; set; } = null!;
        
        [Params(@"Examples\demo_right.json")]
        public override string AfterFile { get; set; } = null!;

        [Params(@"Examples\demo_diff_notext.json")]
        public override string DiffFile { get; set; } = null!;
    }
}
