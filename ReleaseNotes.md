# Release Notes

## 2.0.0

- **[BREAKING CHANGE]** Targeting framework changes:
  - Added: .NET 8, .NET 7, .NET 6, .NET Framework 4.6.2
  - Removed: .NET Standard 2.1, .NET Framework 4.6.1
- **[BREAKING CHANGE]** Minimum version of `System.Text.Json` required is bumped up to `8.0.0`.
- **[BREAKING CHANGE]** When targeting `net8.0`, the following methods are changed to be a wrapper of the methods introduced to `JsonNode` in [this issue](https://github.com/dotnet/runtime/issues/56592):
  - `JsonDiffPatcher.DeepEquals(JsonNode)`
  - `JsonDiffPatcher.DeepClone(JsonNode)`

  Those methods remain unchanged when targeting other frameworks

- **[BREAKING CHANGE]** When targeting `net8.0`, `DeepClone` is no longer an extension method of `JsonNode`. The method is still accessible as static method from `JsonDiffPatcher` type, i.e. `JsonDiffPatcher.DeepClone`
  

## 1.3.1

- Added `PropertyFilter` to `JsonDiffOptions` (#29)
- Fixed bug in diffing null-valued properties (#31)

## 1.3.0

- **Added `DeepEquals` implementation for `JsonDocument` and `JsonElement`**
- Performance improvements in raw text comparison mode
- Removed unnecessary allocation when default diff option is used
- Removed one `DeepEquals` overload that was accidentally exposed as a public method

## 1.2.0

- Major performance improvement in array comparison
- Performance improvements in `DeepEquals` and `JsonValueComparer`
- **[BREAKING CHANGE]** Breaking changes in array comparison:
  - Removed `JsonDiffOptions.PreferFuzzyArrayItemMatch` option
  - `JsonDiffOptions.ArrayItemMatcher` is only invoked when array items are not deeply equal
- **[BREAKING CHANGE]** Base64 encoded text is considered as long text if length is greater than or equal to `JsonDiffOptions.TextDiffMinLength`

## 1.1.0

- Added `JsonValueComparer` that implements semantic comparison of two `JsonValue` objects (including the ones backed by `JsonElement`)
- **[BREAKING CHANGE]** `Diff` method no longer uses `object.Equals` to compare values encapsulated in `JsonValue<T>`. `JsonValueComparer` is used instead
- Added semantic equality to `DeepEquals` method
- Added options to `JsonDiffOptions` to enable semantic diff
- Added `JsonDiffPatcher.DefaultOptions` for customizing default diff options

## 1.0.0

- Initial release