namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class LargeObjectBenchmarks : JsonNetComparisonBenchmark
    {
        public LargeObjectBenchmarks()
            : base(GetExampleFile("large_left.json"),
                GetExampleFile("large_right.json"),
                GetExampleFile("large_diff_notext.json"))
        {
        }
    }
}
