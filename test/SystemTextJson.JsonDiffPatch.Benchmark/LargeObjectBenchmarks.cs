using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class LargeObjectBenchmarks : DiffPatchBenchmarks
    {
        [Params(@"Examples\large_left.json")]
        public override string BeforeFile { get; set; } = null!;
        
        [Params(@"Examples\large_right.json")]
        public override string AfterFile { get; set; } = null!;
        
        [Params(@"Examples\large_diff_notext.json")]
        public override string DiffFile { get; set; } = null!;
    }
}
