using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs
{
    /// <summary>
    /// The type of <see cref="JsonDiffDelta"/>.
    /// </summary>
    public enum DeltaKind
    {
        None,
        Added,
        Modified,
        Deleted,
        Array,
        ArrayMove,
        Object,
        Text
    }
    
    /// <summary>
    /// Implements JSON diff delta format described at <see href="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md"/>.
    /// </summary>
    public struct JsonDiffDelta
    {
        internal const string InvalidPatchDocument = "Invalid patch document.";
        private const int OpTypeDeleted = 0;
        private const int OpTypeTextDiff = 2;
        private const int OpTypeArrayMoved = 3;

        private const string TypePropertyName = "_t";
        private const string ArrayType = "a";

        private JsonNode? _document;

        public JsonDiffDelta(JsonNode document)
        {
            _document = document;
            Kind = GetDeltaKind(document);
        }

        public JsonNode? Document
        {
            get => _document;
            private set
            {
                _document = value;
                Kind = GetDeltaKind(value);
            }
        }

        public DeltaKind Kind { get; private set; }

        private void CheckForKind(DeltaKind expectedKind)
        {
            var kind = Kind;
            if (Kind != expectedKind)
            {
                throw new InvalidOperationException($"Unable to get value from delta of type '{kind}'.");
            }
        }

        public JsonNode? GetAdded()
        {
            CheckForKind(DeltaKind.Added);
            return GetOrClone(Document!.AsArray()[0]);
        }

        public JsonNode? GetDeleted()
        {
            CheckForKind(DeltaKind.Deleted);
            return GetOrClone(Document!.AsArray()[0]);
        }

        public JsonNode? GetNewValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Document!.AsArray()[1]);
        }

        public JsonNode? GetOldValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Document!.AsArray()[0]);
        }

        public int GetNewIndex()
        {
            CheckForKind(DeltaKind.ArrayMove);
            return Document!.AsArray()[1]!.GetValue<int>();
        }

        public string GetTextDiff()
        {
            CheckForKind(DeltaKind.Text);
            return Document!.AsArray()[0]!.GetValue<string>();
        }
        
        public IEnumerable<ArrayChangeEntry> GetArrayChangeEnumerable()
        {
            CheckForKind(DeltaKind.Array);
            foreach (var kvp in Document!.AsObject())
            {
                if (IsTypeProperty(kvp.Key) || !TryGetArrayIndex(kvp.Key, out var index, out _))
                {
                    continue;
                }

                yield return new ArrayChangeEntry(index, kvp.Value!);
            }
        }

        public IEnumerable<ArrayChangeEntry> GetPatchableArrayChangeEnumerable(JsonArray left)
        {
            return GetPatchableArrayChangeEnumerable(left, false);
        }

        internal IEnumerable<ArrayChangeEntry> GetPatchableArrayChangeEnumerable(JsonArray left, bool isReversing)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));
            
            CheckForKind(DeltaKind.Array);

            var arrayPatch = Document!.AsObject();
            var deleteItems = new List<ArrayChangeEntry>(left.Count / 3);
            var addItems = new List<ArrayChangeEntry>(left.Count / 3);
            var patchItems = new List<ArrayChangeEntry>(left.Count / 3);

            // Return items in order:
            // 1. Items to delete
            // 2. Items to add
            // 3. Items to patch
            foreach (var prop in arrayPatch)
            {
                var propertyName = prop.Key;
                if (IsTypeProperty(propertyName))
                {
                    continue;
                }

                var innerPatch = prop.Value;
                if (innerPatch is null)
                {
                    continue;
                }

                if (!TryGetArrayIndex(propertyName, out var index, out var isLeft))
                {
                    throw new FormatException(InvalidPatchDocument);
                }

                var entry = new ArrayChangeEntry(index, innerPatch);
                var kind = entry.Diff.Kind;
                // The left array can only contain deleted or array move operations
                if (isLeft && kind is not DeltaKind.Deleted && kind is not DeltaKind.ArrayMove)
                {
                    throw new FormatException(InvalidPatchDocument);
                }

                if (kind == DeltaKind.Deleted)
                {
                    if (isReversing)
                    {
                        addItems.Add(entry);
                    }
                    else
                    {
                        deleteItems.Add(entry);
                    }
                }
                else if (kind == DeltaKind.ArrayMove)
                {
                    if (isReversing)
                    {
                        var newIndex = entry.Diff.GetNewIndex();
                        if (newIndex < 0 || newIndex >= left.Count)
                        {
                            throw new FormatException(InvalidPatchDocument);
                        }

                        // Delete the item at new index
                        deleteItems.Add(new(newIndex, CreateAdded(null)));
                        // Add it back later at old index
                        addItems.Add(new(index, CreateDeleted(left[newIndex])));
                    }
                    else
                    {
                        if (index < 0 || index >= left.Count)
                        {
                            throw new FormatException(InvalidPatchDocument);
                        }

                        // Delete the item at old index
                        deleteItems.Add(new(index, CreateDeleted(null)));
                        // Add it back later at new index
                        addItems.Add(new(entry.Diff.GetNewIndex(), CreateAdded(left[index])));
                    }
                }
                else if (kind == DeltaKind.Added)
                {
                    if (isReversing)
                    {
                        deleteItems.Add(entry);
                    }
                    else
                    {
                        addItems.Add(entry);
                    }
                }
                else
                {
                    patchItems.Add(entry);
                }
            }

            // Sort items to delete in descending order
            deleteItems.Sort(DescendingCompare);
            // Sort items to add in ascending order
            addItems.Sort(AscendingCompare);

            var enumerable = isReversing
                ? patchItems.Concat(deleteItems).Concat(addItems)
                : deleteItems.Concat(addItems).Concat(patchItems);

            foreach (var kvp in enumerable)
            {
                yield return kvp;
            }

            static int AscendingCompare(ArrayChangeEntry x, ArrayChangeEntry y)
            {
                return x.Index - y.Index;
            }

            static int DescendingCompare(ArrayChangeEntry x, ArrayChangeEntry y)
            {
                return y.Index - x.Index;
            }
        }

        public void Added(JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Added), count: 1);
            var arr = Document!.AsArray();
            arr[0] = newValue?.DeepClone();
        }

        public void Modified(JsonNode? oldValue, JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Modified), count: 2);
            var arr = Document!.AsArray();
            arr[0] = oldValue?.DeepClone();
            arr[1] = newValue?.DeepClone();
        }

        public void Deleted(JsonNode? oldValue)
        {
            EnsureDeltaType(nameof(Deleted), count: 3, opType: OpTypeDeleted);
            var arr = Document!.AsArray();
            arr[0] = oldValue?.DeepClone();
            arr[1] = 0;
        }

        public void ArrayMoveFromDeleted(int newPosition)
        {
            EnsureDeltaType(nameof(ArrayMoveFromDeleted), count: 3, opType: OpTypeDeleted);
            var arr = Document!.AsArray();
            arr[0] = "";
            arr[1] = newPosition;
            arr[2] = OpTypeArrayMoved;
        }
        
        internal void ArrayMoveFromDeleted(int index, int newPosition)
        {
            if (Document is not JsonObject obj)
            {
                return;
            }

            if (!obj.TryGetPropertyValue($"_{index:D}", out var itemDelta)
                || itemDelta is null)
            {
                return;
            }

            var newItemDelta = new JsonDiffDelta(itemDelta);
            newItemDelta.ArrayMoveFromDeleted(newPosition);
        }

        public void ArrayChange(int index, bool isLeft, JsonDiffDelta innerChange)
        {
            if (innerChange.Document is null)
            {
                return;
            }

            var result = innerChange.Document;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ArrayChange), isArrayChange: true);
            var obj = Document!.AsObject();
            obj.Add(isLeft ? $"_{index:D}" : $"{index:D}", result);
        }

        public void ObjectChange(string propertyName, JsonDiffDelta innerChange)
        {
            if (innerChange.Document is null)
            {
                return;
            }

            var result = innerChange.Document;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ObjectChange));
            var obj = Document!.AsObject();
            obj.Add(propertyName, result);
        }

        public void Text(string diff)
        {
            EnsureDeltaType(nameof(Text), count: 3, opType: OpTypeTextDiff);
            var arr = Document!.AsArray();
            arr[0] = diff;
            arr[1] = 0;
            arr[2] = OpTypeTextDiff;
        }

        private void EnsureDeltaType(string opName, int count = 0, int opType = 0,
            bool isArrayChange = false)
        {
            if (count == 0)
            {
                // Object delta, i.e. object and array

                if (Document is null)
                {
                    Document = isArrayChange
                        ? new JsonObject {{TypePropertyName, ArrayType}}
                        : new JsonObject();
                    return;
                }

                if (Document is JsonObject deltaObject)
                {
                    // Check delta object is for array
                    string? deltaType = null;
                    deltaObject.TryGetPropertyValue(TypePropertyName, out var typeNode);
                    // Perf: this is fine we shouldn't have a node backed by JsonElement here
                    typeNode?.AsValue().TryGetValue(out deltaType);

                    if (string.Equals(deltaType, "a") == isArrayChange)
                    {
                        return;
                    }
                }
            }
            else
            {
                // Value delta
                if (Document is null)
                {
                    var newDeltaArray = new JsonArray();
                    for (var i = 0; i < count; i++)
                    {
                        if (i == 2)
                        {
                            newDeltaArray.Add(opType);
                        }
                        else
                        {
                            newDeltaArray.Add(null);
                        }
                    }

                    Document = newDeltaArray;
                    return;
                }

                if (Document is JsonArray deltaArray && deltaArray.Count == count)
                {
                    if (count < 3)
                    {
                        return;
                    }

                    if (deltaArray[count - 1]?.AsValue().GetValue<int>() == opType)
                    {
                        return;
                    }
                }
            }

            throw new InvalidOperationException(
                $"Operation '{opName}' cannot be performed on current delta result.");
        }

        private static DeltaKind GetDeltaKind(JsonNode? delta)
        {
            return delta switch
            {
                JsonArray arr => arr.Count switch
                {
                    1 => DeltaKind.Added,
                    2 => DeltaKind.Modified,
                    3 when arr[2] is JsonValue opType => GetDeltaKindFromOpType(opType),
                    _ => DeltaKind.None
                },
                JsonObject obj => GetDeltaKindFromJsonObject(obj),
                _ => DeltaKind.None
            };

            static DeltaKind GetDeltaKindFromJsonObject(JsonObject obj)
            {
                if (obj.TryGetPropertyValue(TypePropertyName, out var typeParam) &&
                    typeParam is JsonValue typeParamValue &&
                    typeParamValue.TryGetValue<string>(out var typeParamValueStr) &&
                    string.Equals(ArrayType, typeParamValueStr, StringComparison.Ordinal))
                {
                    return DeltaKind.Array;
                }

                return DeltaKind.Object;
            }

            static DeltaKind GetDeltaKindFromOpType(JsonValue opType)
            {
                if (!opType.TryGetValue<int>(out var opTypeValue))
                {
                    return DeltaKind.None;
                }

                return opTypeValue switch
                {
                    OpTypeDeleted => DeltaKind.Deleted,
                    OpTypeArrayMoved => DeltaKind.ArrayMove,
                    OpTypeTextDiff => DeltaKind.Text,
                    _ => DeltaKind.None
                };
            }
        }

        private static JsonNode? GetOrClone(JsonNode? value)
        {
            return value?.Parent is null ? value : value.DeepClone();
        }

        internal static JsonDiffDelta CreateAdded(JsonNode? newValue)
        {
            var delta = new JsonDiffDelta();
            delta.Added(newValue);
            return delta;
        }

        internal static JsonDiffDelta CreateDeleted(JsonNode? oldValue)
        {
            var delta = new JsonDiffDelta();
            delta.Deleted(oldValue);
            return delta;
        }
        
        internal static bool TryGetArrayIndex(string propertyName, out int index, out bool isLeft)
        {
            isLeft = propertyName.StartsWith("_");
            if (int.TryParse(isLeft ? propertyName.Substring(1) : propertyName, out index))
            {
                return true;
            }

            isLeft = false;
            index = 0;
            return false;
        }

        internal static bool IsTypeProperty(string propertyName)
        {
            return string.Equals(TypePropertyName, propertyName);
        }
        
        public readonly struct ArrayChangeEntry
        {
            internal ArrayChangeEntry(int index, JsonNode diff)
            {
                Index = index;
                Diff = new JsonDiffDelta(diff);
            }
            
            internal ArrayChangeEntry(int index, JsonDiffDelta diff)
            {
                Index = index;
                Diff = diff;
            }
            
            public int Index { get; }
            public JsonDiffDelta Diff { get; }
        }
    }
}
