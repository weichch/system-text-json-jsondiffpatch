# SystemTextJson.JsonDiffPatch

![GitHub](https://img.shields.io/github/license/weichch/system-text-json-jsondiffpatch?color=blueviolet) ![GitHub Workflow Status (branch)](https://img.shields.io/github/actions/workflow/status/weichch/system-text-json-jsondiffpatch/build-and-test.yaml?branch=main) [![JsonDiffPatch](https://img.shields.io/nuget/vpre/SystemTextJson.JsonDiffPatch?style=flat)](https://www.nuget.org/packages/SystemTextJson.JsonDiffPatch/) ![Nuget](https://img.shields.io/nuget/dt/SystemTextJson.JsonDiffPatch?color=important)

High-performance, low-allocating JSON object diff and patch extension for System.Text.Json.

## Features

- Compatible with [jsondiffpatch delta format](https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md)
- Support generating patch document in RFC 6902 JSON Patch format
- Support .NET and .NET Framework
- Alternative to [jsondiffpatch.net](https://github.com/wbish/jsondiffpatch.net) which is based on Newtonsoft.Json
- Fast large JSON document diffing with less memory consumption (see [benchmark](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md))
- Support smart array diffing (e.g. move detect) using LCS (Longest Common Subsequence) and custom array item matcher
- _(Only when not using RFC 6902 format)_ Support diffing long text using [google-diff-match-patch](http://code.google.com/p/google-diff-match-patch/), or write your own diff algorithm
- Bonus `DeepEquals` method for comparing `JsonDocument`, `JsonElement` and `JsonNode`
- Bonus `DeepClone` method
- Bonus [`JsonValueComparer`](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/src/SystemTextJson.JsonDiffPatch/JsonValueComparer.cs) that implements semantic comparison of two `JsonValue` objects
- JSON assert for xUnit, MSTest v2 and NUnit with customizable delta output

## Install

#### JsonDiffPatch

```
PM> Install-Package SystemTextJson.JsonDiffPatch
```

#### xUnit Assert

```
PM> Install-Package SystemTextJson.JsonDiffPatch.Xunit
```

#### MSTest v2 Assert

```
PM> Install-Package SystemTextJson.JsonDiffPatch.MSTest
```

#### NUnit Assert

```
PM> Install-Package SystemTextJson.JsonDiffPatch.NUnit
```

## Usage

### Diff

```csharp
// Diff JsonNode
var node1 = JsonNode.Parse("{\"foo\":\"bar\"}");
var node2 = JsonNode.Parse("{\"baz\":\"qux\", \"foo\":\"bar\"}");
var diff = node1.Diff(node2);
// Diff with options
var diff = node1.Diff(node2, new JsonDiffOptions
{
    JsonElementComparison = JsonElementComparison.Semantic
});
// Diff and convert delta into RFC 6902 JSON Patch format
var diff = node1.Diff(node2, new JsonPatchDeltaFormatter());
// Diff JSON files
var diff = JsonDiffPatcher.DiffFile(file1, file2);
// Diff Span<byte>
var diff = JsonDiffPatcher.Diff(span1, span2);
// Diff streams
var diff = JsonDiffPatcher.Diff(stream1, stream2);
// Diff JSON strings
var diff = JsonDiffPatcher.Diff(json1, json2);
// Diff JSON readers
var diff = JsonDiffPatcher.Diff(ref reader1, ref reader2);
```

### Patch & Unpatch

```csharp
var node1 = JsonNode.Parse("{\"foo\":\"bar\"}");
var node2 = JsonNode.Parse("{\"baz\":\"qux\", \"foo\":\"bar\"}");
var diff = node1.Diff(node2);
// In-place patch
JsonDiffPatcher.Patch(ref node1, diff);
// Clone & patch
var patched = node1.PatchNew(diff);
// In-place unpatch
JsonDiffPatcher.ReversePatch(ref node1, diff);
// Clone & unpatch
var patched = node1.ReversePatchNew(diff);
```

### DeepEquals

```csharp
// JsonDocument
var doc1 = JsonDocument.Parse("{\"foo\":1}");
var doc2 = JsonDocument.Parse("{\"foo\":1.0}");
var equal = doc1.DeepEquals(doc2);
var textEqual = doc1.DeepEquals(doc2, JsonElementComparison.RawText);
var semanticEqual = doc1.DeepEquals(doc2, JsonElementComparison.Semantic);

// JsonNode
var node1 = JsonNode.Parse("{\"foo\":1}");
var node2 = JsonNode.Parse("{\"foo\":1.0}");
var equal = node1.DeepEquals(node2);
var textEqual = node1.DeepEquals(node2, JsonElementComparison.RawText);
var semanticEqual = node1.DeepEquals(node2, JsonElementComparison.Semantic);
```

### DeepClone (.NET Framework and .NET 6 & 7)

```csharp
var node = JsonNode.Parse("{\"foo\":\"bar\"}");
var cloned = node.DeepClone();
```

### Default Options

```csharp
// Default diff options
JsonDiffPatcher.DefaultOptions = () => new JsonDiffOptions
{
    JsonElementComparison = JsonElementComparison.Semantic
};

// Default comparison mode for DeepEquals
JsonDiffPatcher.DefaultComparison = JsonElementComparison.Semantic;
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

See detailed [benchmark results](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md).
