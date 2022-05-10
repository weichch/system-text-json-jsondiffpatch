using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal struct JsonNumber
    {
        private long? _longValue;
        private decimal? _decimalValue;
        private double? _doubleValue;
        private readonly float? _floatValue;
        private bool _isValueRead;

        // Keep as field to avoid copy
        public readonly JsonElement Element;

        public JsonNumber(in JsonElement element)
        {
            Debug.Assert(element.ValueKind is JsonValueKind.Number,
                $"Expect JSON element type {JsonValueKind.Number:G}, but found {element.ValueKind:G}.");

            Parent = null;
            HasElement = true;
            Element = element;
            _longValue = null;
            _decimalValue = null;
            _doubleValue = null;
            _floatValue = null;
            _isValueRead = false;
        }

        public JsonNumber(JsonValue parent, int value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        public JsonNumber(JsonValue parent, long value)
            : this(parent)
        {
            _longValue = value;
        }

        public JsonNumber(JsonValue parent, double value)
            : this(parent)
        {
            _doubleValue = value;
        }

        public JsonNumber(JsonValue parent, short value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        public JsonNumber(JsonValue parent, decimal value)
            : this(parent)
        {
            _decimalValue = value;
        }

        public JsonNumber(JsonValue parent, byte value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        public JsonNumber(JsonValue parent, float value)
            : this(parent)
        {
            _floatValue = value;
        }

        public JsonNumber(JsonValue parent, uint value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        public JsonNumber(JsonValue parent, ushort value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        public JsonNumber(JsonValue parent, ulong value)
            : this(parent)
        {
            _decimalValue = Convert.ToDecimal(value);
        }

        public JsonNumber(JsonValue parent, sbyte value)
            : this(parent)
        {
            _longValue = Convert.ToInt64(value);
        }

        private JsonNumber(JsonValue parent)
        {
            Debug.Assert(parent is not null);

            Parent = parent;
            HasElement = false;
            Element = default;
            _longValue = null;
            _decimalValue = null;
            _doubleValue = null;
            _floatValue = null;
            _isValueRead = true;
        }

        public JsonValue? Parent { get; }
        public bool HasElement { get; }

        private long? LongValue
        {
            get
            {
                CreateValue();
                return _longValue;
            }
        }

        private decimal? DecimalValue
        {
            get
            {
                CreateValue();
                return _decimalValue;
            }
        }

        private double? DoubleValue
        {
            get
            {
                CreateValue();
                return _doubleValue;
            }
        }

        private float? FloatValue
        {
            get
            {
                CreateValue();
                return _floatValue;
            }
        }

        private void CreateValue()
        {
            if (_isValueRead)
            {
                return;
            }

            if (HasElement)
            {
                if (Element.TryGetInt64(out var longValue))
                {
                    _longValue = longValue;
                }
                else if (Element.TryGetDecimal(out var decimalValue))
                {
                    _decimalValue = decimalValue;
                }
                else if (Element.TryGetDouble(out var doubleValue))
                {
                    _doubleValue = doubleValue;
                }
            }

            _isValueRead = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal GetDecimal() => DecimalValue ?? (LongValue.HasValue
            ? Convert.ToDecimal(LongValue.Value)
            : DoubleValue.HasValue
                ? Convert.ToDecimal(DoubleValue.Value)
                : Convert.ToDecimal(FloatValue!.Value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetDouble() => DoubleValue ?? (LongValue.HasValue
            ? Convert.ToDouble(LongValue.Value)
            : Convert.ToDouble(FloatValue!.Value));
            
        public int CompareTo(ref JsonNumber another)
        {
            if (DecimalValue.HasValue || another.DecimalValue.HasValue)
            {
                return GetDecimal().CompareTo(another.GetDecimal());
            }

            if (DoubleValue.HasValue || FloatValue.HasValue ||
                another.DoubleValue.HasValue || another.FloatValue.HasValue)
            {
                return JsonValueComparer.CompareDouble(GetDouble(), another.GetDouble());
            }

            return LongValue!.Value.CompareTo(another.LongValue!.Value);
        }

        public bool RawTextEquals(ref JsonNumber another)
        {
            if (!HasElement || HasElement != another.HasElement)
            {
                return false;
            }

            return string.Equals(Element.GetRawText(), another.Element.GetRawText());
        }

        public static bool TryGetJsonNumber(JsonValue jsonValue, out JsonNumber result)
        {
            if (jsonValue.TryGetValue<JsonElement>(out var element))
            {
                if (element.ValueKind is JsonValueKind.Number)
                {
                    result = new JsonNumber(element);
                    return true;
                }
            }
            else
            {
                if (jsonValue.TryGetValue<int>(out var intValue))
                {
                    result = new JsonNumber(jsonValue, intValue);
                    return true;
                }

                if (jsonValue.TryGetValue<long>(out var longValue))
                {
                    result = new JsonNumber(jsonValue, longValue);
                    return true;
                }

                if (jsonValue.TryGetValue<double>(out var doubleValue))
                {
                    result = new JsonNumber(jsonValue, doubleValue);
                    return true;
                }

                if (jsonValue.TryGetValue<short>(out var shortValue))
                {
                    result = new JsonNumber(jsonValue, shortValue);
                    return true;
                }

                if (jsonValue.TryGetValue<decimal>(out var decimalValue))
                {
                    result = new JsonNumber(jsonValue, decimalValue);
                    return true;
                }

                if (jsonValue.TryGetValue<byte>(out var byteValue))
                {
                    result = new JsonNumber(jsonValue, byteValue);
                    return true;
                }

                if (jsonValue.TryGetValue<float>(out var floatValue))
                {
                    result = new JsonNumber(jsonValue, floatValue);
                    return true;
                }

                if (jsonValue.TryGetValue<uint>(out var uintValue))
                {
                    result = new JsonNumber(jsonValue, uintValue);
                    return true;
                }

                if (jsonValue.TryGetValue<ushort>(out var ushortValue))
                {
                    result = new JsonNumber(jsonValue, ushortValue);
                    return true;
                }

                if (jsonValue.TryGetValue<ulong>(out var ulongValue))
                {
                    result = new JsonNumber(jsonValue, ulongValue);
                    return true;
                }

                if (jsonValue.TryGetValue<sbyte>(out var sbyteValue))
                {
                    result = new JsonNumber(jsonValue, sbyteValue);
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
