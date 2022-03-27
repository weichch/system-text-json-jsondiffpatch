using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs.Formatters
{
    public abstract class DefaultDeltaFormatter<TResult> : IJsonDiffDeltaFormatter<TResult>
    {
        private readonly bool _usePatchableArrayChangeEnumerable;

        protected DefaultDeltaFormatter()
            : this(false)
        {
        }

        protected DefaultDeltaFormatter(bool usePatchableArrayChangeEnumerable)
        {
            _usePatchableArrayChangeEnumerable = usePatchableArrayChangeEnumerable;
        }

        public virtual TResult? Format(ref JsonDiffDelta delta, JsonNode? left)
        {
            var value = CreateDefault();
            return FormatJsonDiffDelta(ref delta, left, value);
        }

        protected virtual TResult? CreateDefault()
        {
            return default;
        }

        protected virtual TResult? FormatJsonDiffDelta(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue)
        {
            switch (delta.Kind)
            {
                case DeltaKind.Added:
                    existingValue = FormatAdded(ref delta, existingValue);
                    break;
                case DeltaKind.Modified:
                    existingValue = FormatModified(ref delta, left, existingValue);
                    break;
                case DeltaKind.Deleted:
                    existingValue = FormatDeleted(ref delta, left, existingValue);
                    break;
                case DeltaKind.ArrayMove:
                    existingValue = FormatArrayMove(ref delta, left, existingValue);
                    break;
                case DeltaKind.Text:
                    if (left is not JsonValue leftValue)
                    {
                        throw new FormatException(JsonDiffDelta.InvalidPatchDocument);
                    }
                    
                    existingValue = FormatTextDiff(ref delta, leftValue, existingValue);
                    break;
                case DeltaKind.Array:
                    if (left is not JsonArray leftArray)
                    {
                        throw new FormatException(JsonDiffDelta.InvalidPatchDocument);
                    }

                    existingValue = FormatArray(ref delta, leftArray, existingValue);
                    break;
                case DeltaKind.Object:
                    if (left is not JsonObject leftObject)
                    {
                        throw new FormatException(JsonDiffDelta.InvalidPatchDocument);
                    }

                    existingValue = FormatObject(ref delta, leftObject, existingValue);
                    break;
            }

            return existingValue;
        }

        protected abstract TResult? FormatAdded(ref JsonDiffDelta delta, TResult? existingValue);
        protected abstract TResult? FormatModified(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);
        protected abstract TResult? FormatDeleted(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);
        protected abstract TResult? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);
        protected abstract TResult? FormatTextDiff(ref JsonDiffDelta delta, JsonValue? left, TResult? existingValue);

        protected virtual TResult? FormatArray(ref JsonDiffDelta delta, JsonArray left, TResult? existingValue)
        {
            var arrayChangeEnumerable = _usePatchableArrayChangeEnumerable
                ? delta.GetPatchableArrayChangeEnumerable(left, false)
                : delta.GetArrayChangeEnumerable();

            return arrayChangeEnumerable
                .Aggregate(existingValue, (current, entry) =>
                {
                    var elementDelta = entry.Diff;
                    var leftValue = elementDelta.Kind switch
                    {
                        DeltaKind.Added or DeltaKind.None => null,
                        _ => entry.Index < 0 || entry.Index >= left.Count
                            ? throw new FormatException(JsonDiffDelta.InvalidPatchDocument)
                            : left[entry.Index]
                    };
                    return FormatArrayElement(entry, leftValue, current);
                });
        }

        protected virtual TResult? FormatArrayElement(in JsonDiffDelta.ArrayChangeEntry arrayChange, JsonNode? left, TResult? existingValue)
        {
            var delta = arrayChange.Diff;
            return FormatJsonDiffDelta(ref delta, left, existingValue);
        }

        protected virtual TResult? FormatObject(ref JsonDiffDelta delta, JsonObject left, TResult? existingValue)
        {
            var deltaDocument = delta.Document!.AsObject();
            foreach (var prop in deltaDocument)
            {
                var propDelta = new JsonDiffDelta(prop.Value!);
                left.TryGetPropertyValue(prop.Key, out var leftValue);
                existingValue = FormatObjectProperty(ref propDelta, leftValue, prop.Key, existingValue);
            }

            return existingValue;
        }

        protected virtual TResult? FormatObjectProperty(ref JsonDiffDelta delta, JsonNode? left, string propertyName, TResult? existingValue)
        {
            return FormatJsonDiffDelta(ref delta, left, existingValue);
        }
    }
}