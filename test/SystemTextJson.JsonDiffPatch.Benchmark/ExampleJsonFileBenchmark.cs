using System.IO;
using BenchmarkDotNet.Attributes;

namespace SystemTextJson.JsonDiffPatch.Benchmark
{
    public abstract class ExampleJsonFileBenchmark
    {
        private readonly string _beforeFile;
        private readonly string _afterFile;
        private readonly string _diffFile;

        protected string _jsonBefore = null!;
        protected string _jsonAfter = null!;
        protected string _jsonDiff = null!;
        
        protected ExampleJsonFileBenchmark(string beforeFile, string afterFile, string diffFile)
        {
            _beforeFile = beforeFile;
            _afterFile = afterFile;
            _diffFile = diffFile;
        }

        protected static string GetExampleFile(string fileName)
        {
            return $"Examples/{fileName}";
        }

        [GlobalSetup]
        public virtual void Setup()
        {
            _jsonBefore = File.ReadAllText(_beforeFile);
            _jsonAfter = File.ReadAllText(_afterFile);
            _jsonDiff = File.ReadAllText(_diffFile);
        }
    }
}