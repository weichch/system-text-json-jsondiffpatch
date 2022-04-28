namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public class DemoObjectBenchmarks : JsonNetComparisonBenchmark
    {
        public DemoObjectBenchmarks()
            : base(GetExampleFile("demo_left.json"),
                GetExampleFile("demo_right.json"),
                GetExampleFile("demo_diff_notext.json"))
        {
        }
    }
}
