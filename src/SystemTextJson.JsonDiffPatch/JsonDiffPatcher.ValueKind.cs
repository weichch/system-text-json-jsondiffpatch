using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        /// <summary>
        /// Gets the most significant <see cref="JsonValueKind"/>.
        /// </summary>
        /// <param name="node">The JSON node.</param>
        public static JsonValueKind GetValueKind(this JsonNode? node)
        {
            if (node is null)
            {
                return JsonValueKind.Null;
            }

            if (node is JsonValue value)
            {
                if (value.TryGetValue<JsonElement>(out var element))
                {
                    return element.ValueKind;
                }

                if (value.TryGetValue<int>(out _)
                    || value.TryGetValue<long>(out _)
                    || value.TryGetValue<decimal>(out _)
                    || value.TryGetValue<double>(out _)
                    || value.TryGetValue<short>(out _)
                    || value.TryGetValue<byte>(out _)
                    || value.TryGetValue<float>(out _)
                    || value.TryGetValue<uint>(out _)
                    || value.TryGetValue<ushort>(out _)
                    || value.TryGetValue<ulong>(out _)
                    || value.TryGetValue<sbyte>(out _))
                {
                    return JsonValueKind.Number;
                }

                if (value.TryGetValue<string>(out _)
                    || value.TryGetValue<DateTimeOffset>(out _)
                    || value.TryGetValue<DateTime>(out _)
                    || value.TryGetValue<Guid>(out _)
                    || value.TryGetValue<char>(out _)
                    || value.TryGetValue<byte[]>(out _))
                {
                    return JsonValueKind.String;
                }

                if (value.TryGetValue<bool>(out var booleanValue))
                {
                    return booleanValue ? JsonValueKind.True : JsonValueKind.False;
                }
            }

            if (node is JsonObject)
            {
                return JsonValueKind.Object;
            }

            if (node is JsonArray)
            {
                return JsonValueKind.Array;
            }

            return JsonValueKind.Undefined;
        }
    }
}
