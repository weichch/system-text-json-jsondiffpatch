# Release Notes

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