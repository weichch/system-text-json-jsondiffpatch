using System.Diagnostics;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    /// <summary>
    /// Represents a JSON delta result defined at
    /// <see link="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md"/>.
    /// </summary>
    internal struct JsonDiffDelta
    {
        private const int OpTypeDeleted = 0;
        private const int OpTypeTextDiff = 2;
        private const int OpTypeArrayMoved = 3;

        public JsonDiffDelta(JsonNode delta)
        {
            Result = delta;
        }

        public JsonNode? Result { get; private set; }

        public void Added(JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Added), count: 1);
            var arr = Result!.AsArray();
            arr[0] = newValue.Clone();
        }

        public void Modified(JsonNode? oldValue, JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Modified), count: 2);
            var arr = Result!.AsArray();
            arr[0] = oldValue.Clone();
            arr[1] = newValue.Clone();
        }

        public void Deleted(JsonNode? oldValue)
        {
            EnsureDeltaType(nameof(Deleted), count: 3, opType: OpTypeDeleted);
            var arr = Result!.AsArray();
            arr[0] = oldValue.Clone();
            arr[1] = 0;
        }

        public void ArrayMoveFromDeleted(int newPosition, bool includeOriginalValue)
        {
            EnsureDeltaType(nameof(ArrayMoveFromDeleted), count: 3, opType: OpTypeDeleted);
            var arr = Result!.AsArray();

            if (!includeOriginalValue)
            {
                arr[0] = "";
            }

            arr[1] = newPosition;
            arr[2] = OpTypeArrayMoved;
        }

        public void ArrayChange(int index, bool isLeft, JsonDiffDelta innerChange)
        {
            if (innerChange.Result is null)
            {
                return;
            }

            EnsureDeltaType(nameof(ArrayChange), isArrayChange: true);
            Debug.Assert(innerChange.Result.Parent is null);
            Result!.AsObject().Add(isLeft ? $"_{index:D}" : $"{index:D}", innerChange.Result);
        }

        public void ObjectChange(string propertyName, JsonDiffDelta innerChange)
        {
            if (innerChange.Result is null)
            {
                return;
            }

            EnsureDeltaType(nameof(ObjectChange));
            Debug.Assert(innerChange.Result.Parent is null);
            Result!.AsObject().Add(propertyName, innerChange.Result);
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
                        ? new JsonObject {{"_t", "a"}}
                        : new JsonObject();
                    return;
                }

                if (Result is JsonObject deltaObject)
                {
                    // Check delta object is for array
                    string? deltaType = null;
                    deltaObject.TryGetPropertyValue("_t", out var typeNode);
                    typeNode?.AsValue().TryGetValue<string>(out deltaType);

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

        public static void ChangeDeletedToArrayMoved(JsonDiffDelta delta, int index,
            int newPosition, bool includeOriginalValue)
        {
            if (delta.Result is not JsonObject obj)
            {
                return;
            }

            if (!obj.TryGetPropertyValue($"_{index:D}", out var itemDelta))
            {
                return;
            }

            JsonDiffDelta newItemDelta = itemDelta;
            newItemDelta.ArrayMoveFromDeleted(newPosition, includeOriginalValue);
        }

        public static implicit operator JsonDiffDelta(JsonNode? delta)
        {
            if (delta is null)
            {
                return default;
            }

            return new(delta);
        }
    }
}
