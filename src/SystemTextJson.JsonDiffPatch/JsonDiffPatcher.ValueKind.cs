using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        internal static JsonValueKind GetValueKind(this JsonNode? node, out Type? valueType)
        {
            if (node is null)
            {
                valueType = null;
                return JsonValueKind.Null;
            }

            if (node is JsonValue value)
            {
                if (value.TryGetValue<JsonElement>(out var element))
                {
                    valueType = typeof(JsonElement);
                    return element.ValueKind;
                }

                if (TryGetNumberValueKind(value, out valueType))
                {
                    return JsonValueKind.Number;
                }

                if (TryGetStringValueKind(value, out valueType))
                {
                    return JsonValueKind.String;
                }

                if (value.TryGetValue<bool>(out var booleanValue))
                {
                    valueType = typeof(bool);
                    return booleanValue ? JsonValueKind.True : JsonValueKind.False;
                }
            }
            else if (node is JsonObject)
            {
                valueType = null;
                return JsonValueKind.Object;
            }
            else if (node is JsonArray)
            {
                valueType = null;
                return JsonValueKind.Array;
            }

            valueType = null;
            return JsonValueKind.Undefined;
        }

        private static bool TryGetNumberValueKind(JsonValue value, out Type? valueType)
        {
            if (value.TryGetValue<int>(out _))
            {
                valueType = typeof(int);
                return true;
            }
            
            if (value.TryGetValue<long>(out _))
            {
                valueType = typeof(long);
                return true;
            }
            
            if (value.TryGetValue<double>(out _))
            {
                valueType = typeof(double);
                return true;
            }
            
            if (value.TryGetValue<short>(out _))
            {
                valueType = typeof(short);
                return true;
            }
            
            if (value.TryGetValue<decimal>(out _))
            {
                valueType = typeof(decimal);
                return true;
            }

            if (value.TryGetValue<byte>(out _))
            {
                valueType = typeof(byte);
                return true;
            }

            if (value.TryGetValue<float>(out _))
            {
                valueType = typeof(float);
                return true;
            }

            if (value.TryGetValue<uint>(out _))
            {
                valueType = typeof(uint);
                return true;
            }

            if (value.TryGetValue<ushort>(out _))
            {
                valueType = typeof(ushort);
                return true;
            }

            if (value.TryGetValue<ulong>(out _))
            {
                valueType = typeof(ulong);
                return true;
            }

            if (value.TryGetValue<sbyte>(out _))
            {
                valueType = typeof(sbyte);
                return true;
            }

            valueType = null;
            return false;
        }

        private static bool TryGetStringValueKind(JsonValue value, out Type? valueType)
        {
            if (value.TryGetValue<string>(out _))
            {
                valueType = typeof(string);
                return true;
            }
            
            if (value.TryGetValue<DateTime>(out _))
            {
                valueType = typeof(DateTime);
                return true;
            }
            
            if (value.TryGetValue<DateTimeOffset>(out _))
            {
                valueType = typeof(DateTimeOffset);
                return true;
            }

            if (value.TryGetValue<Guid>(out _))
            {
                valueType = typeof(Guid);
                return true;
            }

            if (value.TryGetValue<char>(out _))
            {
                valueType = typeof(char);
                return true;
            }

            if (value.TryGetValue<byte[]>(out _))
            {
                valueType = typeof(byte[]);
                return true;
            }

            valueType = null;
            return false;
        }
    }
}
