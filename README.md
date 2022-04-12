# SystemTextJson.JsonDiffPatch

High-performance, low-allocating JSON objects diff and patch extension for System.Text.Json.

## Features

- Use [jsondiffpatch](https://github.com/benjamine/jsondiffpatch) delta format described [here](https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md)
- Support generating patch document in RFC 6902 JSON Patch format
- Target latest .NET Standard and .NET Framework 4.6.1 (for legacy apps) and leverage latest .NET features
- Alternative to [jsondiffpatch.net](https://github.com/wbish/jsondiffpatch.net) which is based on Newtonsoft.Json
- Fast large JSON document diffing with less memory consumption (see [benchmark](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md))
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

[Benchmark results](https://github.com/weichch/system-text-json-jsondiffpatch/blob/main/Benchmark.md) were generated using example objects [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/Examples) and benchmark tests [here](https://github.com/weichch/system-text-json-jsondiffpatch/tree/main/test/SystemTextJson.JsonDiffPatch.Benchmark/).