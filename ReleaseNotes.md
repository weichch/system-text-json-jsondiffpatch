# Release Notes

## 1.1.0
- Added `JsonValueComparer` that implements semantic comparison of two `JsonValue` objects (including the ones backed by `JsonElement`)
- **[BREAKING CHANGE]** `Diff` method no longer uses `object.Equals` to compare values encapsulated in `JsonValue<T>`. `JsonValueComparer` is used instead.
- Added semantic equality to `DeepEquals` method
- Added options to `JsonDiffOptions` to enable semantic diff

## 1.0.0
- Initial release