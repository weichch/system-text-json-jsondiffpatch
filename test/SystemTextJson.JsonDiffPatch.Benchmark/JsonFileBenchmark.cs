using System.IO;
using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    [IterationCount(50)]
    public abstract class JsonFileBenchmark
    {
        [ParamsAllValues]
        public virtual JsonFileSize FileSize { get; set; }

        protected string JsonLeft { get; set; } = null!;
        protected string JsonRight { get; set; } = null!;
        protected string JsonDiff { get; set; } = null!;

        [GlobalSetup]
        public virtual void Setup()
        {
            JsonLeft = File.ReadAllText(GetFilePath(FileSize, "left"));
            JsonRight = File.ReadAllText(GetFilePath(FileSize, "right"));
            JsonDiff = File.ReadAllText(GetFilePath(FileSize, "diff"));

            static string GetFilePath(JsonFileSize fileSize, string suffix)
            {
                var fileName = $"{fileSize:G}_{suffix}.json";
                return $"Examples/{fileName.ToLowerInvariant()}";
            }
        }

        public enum JsonFileSize
        {
            Small,
            Large
        }
    }
}