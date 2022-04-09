# system-text-json-jsondiffpatch

High-performance, low-allocating JSON objects diff and patch extension for System.Text.Json.

## Features

- Use [jsondiffpatch](https://github.com/benjamine/jsondiffpatch) delta format described [here](https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md)
- Support generating patch document in RFC 6902 JSON Patch format
- Target latest .NET Standard and .NET Framework 4.6.1 (for legacy apps) and leverage latest .NET features
- Alternative to [jsondiffpatch.net](https://github.com/wbish/jsondiffpatch.net) which is based on Newtonsoft.Json
- Fast large JSON document diffing with less memory consumption
- Support smart array diffing (e.g. move detect) using LCS and custom array item matcher
- _(Only when not using RFC 6902 format)_ Support diffing long text using [google-diff-match-patch](http://code.google.com/p/google-diff-match-patch/), or write your own diff algorithm
- Bonus `JsonNode.DeepClone` and `JsonNode.DeepEquals` methods
- JSON assert for xUnit, MSTest v2 and NUnit with customizable delta output

## Install

| JsonDiffPatch                                                                                        | xUnit Assert                                                                                              | MSTest v2 Assert                                                                                               | NUnit Assert                                                                                              |
|------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| [![JsonDiffPatch](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch?style=for-the-badge)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch/) | [![xUnit Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.Xunit?style=for-the-badge)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.Xunit/) | [![MSTest v2 Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.MSTest?style=for-the-badge)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.MSTest/) | [![NUnit Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.NUnit?style=for-the-badge)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.NUnit/) |

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
// Diff JsonNode objects
var node1 = JsonNode.Parse(...);
var node2 = JsonNode.Parse(...);
JsonNode? diff = node1.Diff(node2);
// Diff with options
JsonNode? diff = node1.Diff(node2, new JsonDiffOptions { ... });
// Diff and convert delta into RFC 6902 JSON Patch format
JsonNode? diff = node1.Diff(node2, new JsonPatchDeltaFormatter());
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

### Patch & Unpatch

```csharp
var node1 = JsonNode.Parse(...);
var node2 = JsonNode.Parse(...);
JsonNode? diff = node1.Diff(node2);
// In-place patch
JsonDiffPatcher.Patch(ref node1, diff);
// Clone & patch
node1.PatchNew(diff);
// In-place unpatch
JsonDiffPatcher.ReversePatch(ref node1, diff);
// Clone & unpatch
node1.ReversePatchNew(diff);
```

### Assert (Unit Testing)

```csharp
var expected = JsonNode.Parse(...);
var actual = JsonNode.Parse(...);

// xUnit
JsonAssert.Equal(expected, actual);
actual.ShouldEqual(expected);
JsonAssert.NotEqual(expected, actual);
actual.ShouldNotEqual(expected);

// MSTest
JsonAssert.AreEqual(expected, actual);
Assert.That.JsonAreEqual(expected, actual);
JsonAssert.AreNotEqual(expected, actual);
Assert.That.JsonAreNotEqual(expected, actual);

// NUnit
JsonAssert.AreEqual(expected, actual);
Assert.That(actual, JsonIs.EqualTo(expected));
JsonAssert.AreNotEqual(expected, actual);
Assert.That(actual, JsonIs.NotEqualTo(expected));
```

Example output _(when output is enabled)_:
```
JsonAssert.Equal() failure.
Expected:
{
  "foo": "baz"
}
Actual:
{
  "foo": "bar",
  "baz": "qux"
}
Delta:
{
  "foo": [
    "baz",
    "bar"
  ],
  "baz": [
    "qux"
  ]
}
```

## Benchmark

Benchmarks were generated using example objects [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/Examples) and benchmark tests [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/SystemTextJson.JsonDiffPatch.Benchmark/).

### Demo JSON object from jsondiffpatch

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1415 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |      Mean |       Min |       Max |       P95 |       P80 | Allocated |
|-------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
|              Diff_JsonNet | 107.55 μs |  95.29 μs | 122.73 μs | 118.01 μs | 113.43 μs |    132 KB |
|       Diff_SystemTextJson |  91.79 μs |  75.05 μs | 126.17 μs | 112.61 μs | 101.81 μs |     70 KB |
|          Diff_JsonNet_Rfc | 130.02 μs | 115.41 μs | 156.85 μs | 151.16 μs | 136.42 μs |    150 KB |
|   Diff_SystemTextJson_Rfc | 106.12 μs |  95.75 μs | 120.03 μs | 115.57 μs | 110.23 μs |     93 KB |
|             Patch_JsonNet | 116.92 μs | 107.06 μs | 137.86 μs | 133.26 μs | 122.34 μs |    162 KB |
|      Patch_SystemTextJson |  45.05 μs |  37.98 μs |  56.60 μs |  55.13 μs |  47.42 μs |     37 KB |
|        DeepEquals_JsonNet |  69.14 μs |  62.38 μs |  76.96 μs |  74.78 μs |  71.95 μs |     91 KB |
| DeepEquals_SystemTextJson |  53.43 μs |  47.96 μs |  65.03 μs |  60.30 μs |  56.35 μs |     40 KB |
|         DeepClone_JsonNet |  51.28 μs |  44.65 μs |  62.34 μs |  58.69 μs |  54.52 μs |     70 KB |
|  DeepClone_SystemTextJson |  47.82 μs |  38.78 μs |  59.26 μs |  56.94 μs |  51.11 μs |     45 KB |


### Large JSON object

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1415 (21H1/May2021Update)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |      Mean |       Min |       Max |       P95 |       P80 | Allocated |
|-------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
|              Diff_JsonNet | 11.155 ms |  9.132 ms | 13.732 ms | 12.802 ms | 11.896 ms |      4 MB |
|       Diff_SystemTextJson |  8.375 ms |  7.334 ms |  9.132 ms |  8.978 ms |  8.690 ms |      3 MB |
|          Diff_JsonNet_Rfc | 12.774 ms |  9.807 ms | 17.990 ms | 17.479 ms | 13.374 ms |      6 MB |
|   Diff_SystemTextJson_Rfc | 11.664 ms | 10.341 ms | 12.918 ms | 12.668 ms | 12.142 ms |      5 MB |
|             Patch_JsonNet | 12.146 ms | 10.344 ms | 13.576 ms | 13.302 ms | 12.647 ms |      5 MB |
|      Patch_SystemTextJson |  4.693 ms |  2.422 ms |  5.685 ms |  5.412 ms |  5.061 ms |      2 MB |
|        DeepEquals_JsonNet |  4.164 ms |  2.584 ms |  5.424 ms |  4.917 ms |  4.652 ms |      2 MB |
| DeepEquals_SystemTextJson |  3.254 ms |  2.071 ms |  3.837 ms |  3.709 ms |  3.499 ms |      2 MB |
|         DeepClone_JsonNet |  3.997 ms |  2.529 ms |  5.165 ms |  4.772 ms |  4.531 ms |      2 MB |
|  DeepClone_SystemTextJson |  3.396 ms |  2.085 ms |  3.981 ms |  3.811 ms |  3.609 ms |      2 MB |

