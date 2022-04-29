# SystemTextJson.JsonDiffPatch

![GitHub](https://img.shields.io/github/license/weichch/system-text-json-jsondiffpatch?color=blueviolet) ![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/weichch/system-text-json-jsondiffpatch/build-and-test/main) ![Nuget](https://img.shields.io/nuget/dt/SystemTextJson.JsonDiffPatch?color=important)

High-performance, low-allocating JSON objects diff and patch extension for System.Text.Json.

## Features

- Compatible with [jsondiffpatch delta format](https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md)
- Support generating patch document in RFC 6902 JSON Patch format
- Target latest .NET Standard and .NET Framework 4.6.1 (for legacy apps) and leverage latest .NET features
- Alternative to [jsondiffpatch.net](https://github.com/wbish/jsondiffpatch.net) which is based on Newtonsoft.Json
- Fast large JSON document diffing with less memory consumption (see [benchmark](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md))
- Support smart array diffing (e.g. move detect) using LCS (Longest Common Subsequence) and custom array item matcher
- _(Only when not using RFC 6902 format)_ Support diffing long text using [google-diff-match-patch](http://code.google.com/p/google-diff-match-patch/), or write your own diff algorithm
- Bonus `JsonNode.DeepClone` and `JsonNode.DeepEquals` methods
- Bouns [`JsonValueComparer`](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/src/SystemTextJson.JsonDiffPatch/JsonValueComparer.cs) that implements semantic comparison of two `JsonValue` objects (including `JsonValue` backed by `JsonElement`)
- JSON assert for xUnit, MSTest v2 and NUnit with customizable delta output

## Install

|                                                                        JsonDiffPatch                                                                        |                                                                              xUnit Assert                                                                              |                                                                               MSTest v2 Assert                                                                               |                                                                              NUnit Assert                                                                              |
|:-----------------------------------------------------------------------------------------------------------------------------------------------------------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [![JsonDiffPatch](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch?style=flat)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch/) | [![xUnit Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.Xunit?style=flat)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.Xunit/) | [![MSTest v2 Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.MSTest?style=flat)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.MSTest/) | [![NUnit Assert](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch.NUnit?style=flat)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch.NUnit/) |

## Usage

### Diff

```csharp
// Diff JsonNode
var node1 = JsonNode.Parse("{\"foo\":\"bar\"}");
var node2 = JsonNode.Parse("{\"baz\":\"qux\", \"foo\":\"bar\"}");
JsonNode? diff = node1.Diff(node2);
// Diff with options
JsonNode? diff = node1.Diff(node2, new JsonDiffOptions
{
    JsonElementComparison = JsonElementComparison.Semantic
});
// Diff and convert delta into RFC 6902 JSON Patch format
JsonNode? diff = node1.Diff(node2, new JsonPatchDeltaFormatter());
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
```

### DeepClone

```csharp
var node = JsonNode.Parse("{\"foo\":\"bar\"}");
JsonNode? cloned = node.DeepClone();
```

### DeepEquals

```csharp
var node1 = JsonNode.Parse("{\"foo\":1.0}");
var node2 = JsonNode.Parse("{\"foo\":1}");
// equal is false
bool equal = node1.DeepEquals(node2);
// semanticEqual is true
bool semanticEqual = node1.DeepEquals(node2, JsonElementComparison.Semantic);
```

### Semantic Value Comparison
```csharp
var node1 = JsonNode.Parse("\"2019-11-27\"");
var node2 = JsonNode.Parse("\"2019-11-27T00:00:00.000\"");
// dateCompare is 0
var dateCompare = JsonValueComparer.Compare(node1, node2);

var node3 = JsonNode.Parse("1");
var node4 = JsonNode.Parse("1.00");
// numCompare is 0
var numCompare = JsonValueComparer.Compare(node3, node4);
```

### Patch & Unpatch

```csharp
var node1 = JsonNode.Parse("{\"foo\":\"bar\"}");
var node2 = JsonNode.Parse("{\"baz\":\"qux\", \"foo\":\"bar\"}");
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

### Default Options

```csharp
JsonDiffPatcher.DefaultOptions = () => new JsonDiffOptions
{
    JsonElementComparison = JsonElementComparison.Semantic
};
```

### Assert (Unit Testing)

```csharp
var expected = JsonNode.Parse("{\"foo\":\"bar\"}");
var actual = JsonNode.Parse("{\"baz\":\"qux\", \"foo\":\"bar3\"}");

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
  "foo": "bar"
}
Actual:
{
  "baz": "qux",
  "foo": "bar"
}
Delta:
{
  "foo": [
    "bar",
    "bar3"
  ],
  "baz": [
    "qux"
  ]
}
```

## Benchmark

[Benchmark results](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md) were generated using example objects [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/Examples) and benchmark tests [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/SystemTextJson.JsonDiffPatch.Benchmark/).