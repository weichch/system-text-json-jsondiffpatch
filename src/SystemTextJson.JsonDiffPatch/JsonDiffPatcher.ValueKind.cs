﻿using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        internal static JsonValueKind GetValueKind(this JsonNode? node, out Type? valueType, out bool isJsonElement)
        {
            isJsonElement = false;
            
            if (node is null)
            {
                valueType = null;
                return JsonValueKind.Null;
            }

            if (node is JsonValue value)
            {
                if (value.TryGetValue<JsonElement>(out var element))
                {
                    valueType = GetElementValueType(element);
                    isJsonElement = true;
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

        private static Type? GetElementValueType(in JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out _))
                        return typeof(long);
                    if (element.TryGetDecimal(out _))
                        return typeof(decimal);
                    if (element.TryGetDouble(out _))
                        return typeof(double);

                    throw new ArgumentException("Unsupported JSON number.");

                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out _))
                        return typeof(DateTimeOffset);
                    if (element.TryGetDateTime(out _))
                        return typeof(DateTime);
                    if (element.TryGetGuid(out _))
                        return typeof(Guid);
                    
                    // Don't test base64 here, it's too expensive to test

                    return typeof(string);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return typeof(bool);

                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                default:
                    throw new ArgumentOutOfRangeException(nameof(element.ValueKind),
                        $"Unexpected value kind {element.ValueKind:G}");
            }
        }
    }
}
