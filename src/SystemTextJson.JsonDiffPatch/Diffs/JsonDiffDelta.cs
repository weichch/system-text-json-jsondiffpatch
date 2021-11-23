using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    /// <summary>
    /// The type of delta.
    /// </summary>
    internal enum DeltaKind
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

    // https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md
    internal struct JsonDiffDelta
    {
        private const int OpTypeDeleted = 0;
        private const int OpTypeTextDiff = 2;
        private const int OpTypeArrayMoved = 3;

        private const string TypePropertyName = "_t";
        private const string ArrayType = "a";

        private JsonNode? _result;

        public JsonDiffDelta(JsonNode delta)
        {
            _result = delta;
            Kind = GetDeltaKind(delta);
        }

        public JsonNode? Result
        {
            get => _result;
            private set
            {
                _result = value;
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
            return GetOrClone(Result!.AsArray()[0]);
        }

        public JsonNode? GetDeleted()
        {
            CheckForKind(DeltaKind.Deleted);
            return GetOrClone(Result!.AsArray()[0]);
        }

        public JsonNode? GetNewValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Result!.AsArray()[1]);
        }

        public JsonNode? GetOldValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Result!.AsArray()[0]);
        }

        public int GetNewIndex()
        {
            CheckForKind(DeltaKind.ArrayMove);
            return Result!.AsArray()[1]!.GetValue<int>();
        }

        public void Added(JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Added), count: 1);
            var arr = Result!.AsArray();
            arr[0] = newValue.DeepClone();
        }

        public void Modified(JsonNode? oldValue, JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Modified), count: 2);
            var arr = Result!.AsArray();
            arr[0] = oldValue.DeepClone();
            arr[1] = newValue.DeepClone();
        }

        public void Deleted(JsonNode? oldValue)
        {
            EnsureDeltaType(nameof(Deleted), count: 3, opType: OpTypeDeleted);
            var arr = Result!.AsArray();
            arr[0] = oldValue.DeepClone();
            arr[1] = 0;
        }

        public void ArrayMoveFromDeleted(int newPosition)
        {
            EnsureDeltaType(nameof(ArrayMoveFromDeleted), count: 3, opType: OpTypeDeleted);
            var arr = Result!.AsArray();
            arr[0] = "";
            arr[1] = newPosition;
            arr[2] = OpTypeArrayMoved;
        }

        public void ArrayChange(int index, bool isLeft, JsonDiffDelta innerChange)
        {
            if (innerChange.Result is null)
            {
                return;
            }

            var result = innerChange.Result;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ArrayChange), isArrayChange: true);
            var obj = Result!.AsObject();
            obj.Add(isLeft ? $"_{index:D}" : $"{index:D}", result);
        }

        public void ObjectChange(string propertyName, JsonDiffDelta innerChange)
        {
            if (innerChange.Result is null)
            {
                return;
            }

            var result = innerChange.Result;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ObjectChange));
            var obj = Result!.AsObject();
            obj.Add(propertyName, result);
        }

        public void Text(string diff)
        {
            EnsureDeltaType(nameof(Text), count: 3, opType: OpTypeTextDiff);
            var arr = Result!.AsArray();
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

                if (Result is null)
                {
                    Result = isArrayChange
                        ? new JsonObject {{TypePropertyName, ArrayType}}
                        : new JsonObject();
                    return;
                }

                if (Result is JsonObject deltaObject)
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
                if (Result is null)
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

                    Result = newDeltaArray;
                    return;
                }

                if (Result is JsonArray deltaArray && deltaArray.Count == count)
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

        public static JsonDiffDelta CreateAdded(JsonNode? newValue)
        {
            var delta = new JsonDiffDelta();
            delta.Added(newValue);
            return delta;
        }

        public static JsonDiffDelta CreateDeleted(JsonNode? oldValue)
        {
            var delta = new JsonDiffDelta();
            delta.Deleted(oldValue);
            return delta;
        }

        public static void ChangeDeletedToArrayMoved(JsonDiffDelta delta, int index, int newPosition)
        {
            if (delta.Result is not JsonObject obj)
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

        public static bool TryGetArrayIndex(string propertyName, out int index, out bool isLeft)
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

        public static bool IsTypeProperty(string propertyName)
        {
            return string.Equals(TypePropertyName, propertyName);
        }
    }
}
