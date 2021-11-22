# system-text-json-jsondiffpatch

High-performance, low-allocating JSON objects diff and patch extension for `System.Text.Json`.

## Features

- Use [jsondiffpatch](https://github.com/benjamine/jsondiffpatch) delta format described [here](https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md)
- Target `.NET Standard 2.0` and leverage latest .NET features
- Similar diff experience as [jsondiffpatch.net](https://github.com/wbish/jsondiffpatch.net) (based on Newtonsoft.Json)
- Fast large JSON document diffing with less memory consumption
- Support smart array diffing (e.g. move detect) using LCS and custom array item matcher
- Support diffing long text using [google-diff-match-patch](http://code.google.com/p/google-diff-match-patch/), or write your own diff algorithm
- `JsonNode.DeepClone` and `JsonNode.DeepEquals` methods

- (_Under development_) Patch, unpatch, formatters etc

## Benchmark

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.631 (20H2/October2020Update)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.403
  [Host]     : .NET 5.0.12 (5.0.1221.52207), X64 RyuJIT
  DefaultJob : .NET 5.0.12 (5.0.1221.52207), X64 RyuJIT


```
|                     Method |        Mean |         Min |         Max |         P95 |         P80 | Allocated |
|--------------------------- |------------:|------------:|------------:|------------:|------------:|----------:|
|         DemoObject_JsonNet |    165.4 μs |    158.4 μs |    168.6 μs |    168.0 μs |    167.4 μs |    173 KB |
|  DemoObject_DefaultOptions |    152.8 μs |    146.9 μs |    160.1 μs |    159.3 μs |    155.3 μs |     84 KB |
|     DemoObject_NoArrayMove |    152.0 μs |    147.4 μs |    154.0 μs |    153.6 μs |    153.2 μs |     84 KB |
|         DemoObject_Mutable |    104.1 μs |    101.6 μs |    106.1 μs |    106.0 μs |    105.1 μs |     70 KB |
|        LargeObject_JsonNet | 89,434.7 μs | 84,515.2 μs | 92,858.6 μs | 92,387.7 μs | 91,774.2 μs | 23,628 KB |
| LargeObject_DefaultOptions |  7,446.1 μs |  7,156.2 μs |  7,794.0 μs |  7,775.8 μs |  7,518.6 μs |  4,085 KB |
|    LargeObject_NoArrayMove |  7,364.3 μs |  7,072.5 μs |  7,575.8 μs |  7,530.3 μs |  7,472.6 μs |  4,087 KB |
|        LargeObject_Mutable |  6,699.4 μs |  6,400.8 μs |  7,017.8 μs |  6,935.1 μs |  6,804.6 μs |  3,538 KB |


_\* Generated using example objects [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/Examples) and benchmark tests [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/SystemTextJson.JsonDiffPatch.Benchmark/SimpleDiffBenchmark.cs)_

# Install

Install from [NuGet.org](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch/):

```
Install-Package SystemTextJson.JsonDiffPatch
```

## Usage
### Diff

```csharp
// Diff JSON files
JsonNode? diff = JsonDiffPatcher.DiffFile(file1, file2);
// Diff Span<byte>
JsonNode? diff = JsonDiffPatcher.Diff(span1, span2);
// Diff streams
JsonNode? diff = JsonDiffPatcher.Diff(stream1, stream2);
// Diff JSON strings
JsonNode? diff = JsonDiffPatcher.Diff(json1, json2);
// Diff JSON readers
JsonNode? diff = JsonDiffPatcher.Diff(ref reader1, ref reader2);

// Diff mutable JsonNode objects
var node1 = JsonNode.Parse(...);
var node2 = JsonNode.Parse(...);
JsonNode? diff = node1.Diff(node2);
```

### DeepClone

```csharp
var node = JsonNode.Parse(...);
JsonNode? cloned = node.DeepClone();
```

### DeepEquals

```csharp
var node1 = JsonNode.Parse(...);
var node2 = JsonNode.Parse(...);
bool equal = node1.DeepEquals(node2);
```

### Options

```csharp
public struct JsonDiffOptions
{
    /// <summary>
    /// Specifies whether to suppress detect array move. Default value is <c>false</c>.
    /// </summary>
    public bool SuppressDetectArrayMove { get; set; }

    /// <summary>
    /// Gets or sets the function to match array items.
    /// </summary>
    public ArrayItemMatch? ArrayItemMatcher { get; set; }

    /// <summary>
    /// Gets or sets the function to find key of a <see cref="JsonObject"/>
    /// or <see cref="JsonArray"/>. This is used when matching array items by
    /// their keys. If this function returns <c>null</c>, the items being
    /// compared are treated as "not keyed". When comparing two "not keyed"
    /// objects, their contents are compared. This function is only used when
    /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>.
    /// </summary>
    public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; set; }

    /// <summary>
    /// Gets or sets whether two instances of JSON object types (object and array)
    /// are considered equal if their position is the same in their parent
    /// arrays regardless of their contents. This property is only used when
    /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>. By settings this
    /// property to <c>true</c>, a diff could be returned faster but larger in
    /// size. Default value is <c>false</c>.
    /// </summary>
    public bool ArrayObjectItemMatchByPosition { get; set; }

    /// <summary>
    /// Gets or sets whether to prefer <see cref="ArrayObjectItemKeyFinder"/> and
    /// <see cref="ArrayObjectItemMatchByPosition"/> than using deep value comparison
    /// to match array object items. By settings this property to <c>true</c>,
    /// a diff could be returned faster but larger in size. Default value is <c>false</c>.
    /// </summary>
    public bool PreferFuzzyArrayItemMatch { get; set; }

    /// <summary>
    /// Gets or sets the minimum length for diffing texts using <see cref="TextMatcher"/>
    /// or default text diffing algorithm, aka Google's diff-match-patch algorithm. When text
    /// diffing algorithm is not used, text diffing is fallback to value replacement. If this
    /// property is set to <c>0</c>, diffing algorithm is disabled. Default value is <c>0</c>.
    /// </summary>
    public int TextDiffMinLength { get; set; }

    /// <summary>
    /// Gets or sets the function to match long texts.
    /// </summary>
    public TextMatch? TextMatcher { get; set; }
}
```
