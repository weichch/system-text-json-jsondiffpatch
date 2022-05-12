using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    internal struct JsonString
    {
        internal static readonly JsonSerializerOptions SerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private DateTime? _dateTimeValue;
        private DateTimeOffset? _dateTimeOffsetValue;
        private Guid? _guidValue;
        private readonly char? _charValue;
        private readonly byte[]? _byteArrayValue;
        private readonly string? _stringValue;
        private JsonStringValueKind _stringKind;
        private bool _isValueRead;

        // Keep as field to avoid copy
        public readonly JsonElement Element;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(in JsonElement element)
        {
            Debug.Assert(element.ValueKind is JsonValueKind.String,
                $"Expect JSON element type {JsonValueKind.String:G}, but found {element.ValueKind:G}.");

            Parent = null;
            HasElement = true;
            Element = element;
            _dateTimeValue = null;
            _dateTimeOffsetValue = null;
            _guidValue = null;
            _charValue = null;
            _byteArrayValue = null;
            _stringValue = null;
            _stringKind = default;
            _isValueRead = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, DateTime value)
            : this(parent, JsonStringValueKind.DateTime)
        {
            _dateTimeValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, DateTimeOffset value)
            : this(parent, JsonStringValueKind.DateTime)
        {
            _dateTimeOffsetValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, Guid value)
            : this(parent, JsonStringValueKind.Guid)
        {
            _guidValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, char value)
            : this(parent, JsonStringValueKind.String)
        {
            _charValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, byte[] value)
            : this(parent, JsonStringValueKind.String)
        {
            _byteArrayValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonString(JsonValue parent, string value)
            : this(parent, JsonStringValueKind.String)
        {
            _stringValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonString(JsonValue parent, JsonStringValueKind stringKind)
        {
            Debug.Assert(parent is not null);

            Parent = parent;
            HasElement = false;
            Element = default;
            _dateTimeValue = null;
            _dateTimeOffsetValue = null;
            _guidValue = null;
            _charValue = null;
            _byteArrayValue = null;
            _stringValue = null;
            _stringKind = stringKind;
            _isValueRead = true;
        }

        public JsonValue? Parent { get; }
        public bool HasElement { get; }

        private DateTime? DateTimeValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _dateTimeValue;
            }
        }

        private DateTimeOffset? DateTimeOffsetValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _dateTimeOffsetValue;
            }
        }

        private Guid? GuidValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _guidValue;
            }
        }

        private char? CharValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _charValue;
            }
        }

        private byte[]? ByteArrayValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _byteArrayValue;
            }
        }

        private string? StringValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _stringValue;
            }
        }

        public JsonStringValueKind StringKind
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CreateValue();
                return _stringKind;
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
                if (Element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
                {
                    _dateTimeOffsetValue = dateTimeOffsetValue;
                    _stringKind = JsonStringValueKind.DateTime;
                }
                else if (Element.TryGetDateTime(out var dateTimeValue))
                {
                    _dateTimeValue = dateTimeValue;
                    _stringKind = JsonStringValueKind.DateTime;
                }
                else if (Element.TryGetGuid(out var guidValue))
                {
                    _guidValue = guidValue;
                    _stringKind = JsonStringValueKind.Guid;
                }
            }

            // We don't read string value here as string comparison can be optimized using
            // ValueEquals rather than string.Equals
            _isValueRead = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime GetDateTime() => DateTimeValue ?? DateTimeOffsetValue!.Value.DateTime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTimeOffset GetDateTimeOffset() => DateTimeOffsetValue ?? new DateTimeOffset(DateTimeValue!.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string? GetString() => StringValue ?? CharValue?.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string? GetJsonString() => Parent is not null
            ? Parent.ToJsonString(SerializerOptions)
            : HasElement
                ? JsonSerializer.Serialize(Element, SerializerOptions)
                : null;

        public int CompareTo(ref JsonString another)
        {
            if (StringKind != another.StringKind)
            {
                return StringComparer.Ordinal.Compare(GetJsonString(), another.GetJsonString());
            }

            switch (StringKind)
            {
                case JsonStringValueKind.DateTime:
                    return DateTimeValue.HasValue
                        ? GetDateTime().CompareTo(another.GetDateTime())
                        : GetDateTimeOffset().CompareTo(another.GetDateTimeOffset());

                case JsonStringValueKind.Guid:
                    return GuidValue!.Value.CompareTo(another.GuidValue!.Value);

                case JsonStringValueKind.String:
                    if (HasElement && another.HasElement)
                    {
                        return StringComparer.Ordinal.Compare(Element.GetString(), another.Element.GetString());
                    }

                    if ((StringValue is not null || CharValue.HasValue) &&
                        (another.StringValue is not null || another.CharValue.HasValue))
                    {
                        return StringComparer.Ordinal.Compare(GetString(), another.GetString());
                    }

                    if (ByteArrayValue is not null && another.ByteArrayValue is not null)
                    {
                        return JsonValueComparer.CompareByteArray(ByteArrayValue, another.ByteArrayValue);
                    }

                    return StringComparer.Ordinal.Compare(GetJsonString(), another.GetJsonString());
                default:
                    throw new ArgumentOutOfRangeException(nameof(StringKind), $"Unexpected string kind {StringKind}.");
            }
        }

        public bool Equals(ref JsonString another)
        {
            if (StringKind != another.StringKind)
            {
                return false;
            }

            switch (StringKind)
            {
                case JsonStringValueKind.DateTime:
                    return DateTimeValue.HasValue
                        ? GetDateTime().Equals(another.GetDateTime())
                        : GetDateTimeOffset().Equals(another.GetDateTimeOffset());

                case JsonStringValueKind.Guid:
                    return GuidValue!.Value.Equals(another.GuidValue!.Value);

                case JsonStringValueKind.String:
                    if (HasElement || another.HasElement)
                    {
                        if (HasElement && another.HasElement)
                        {
                            return Element.ValueEquals(another.Element.GetString());
                        }

                        if (HasElement && (another.StringValue is not null || another.CharValue.HasValue))
                        {
                            if (another.CharValue.HasValue)
                            {
                                Span<char> buf = stackalloc char[1];
                                buf[0] = another.CharValue.Value;
                                return Element.ValueEquals(buf);
                            }

                            return Element.ValueEquals(another.StringValue);
                        }

                        if (another.HasElement && (StringValue is not null || CharValue.HasValue))
                        {
                            if (CharValue.HasValue)
                            {
                                Span<char> buf = stackalloc char[1];
                                buf[0] = CharValue.Value;
                                return another.Element.ValueEquals(buf);
                            }

                            return another.Element.ValueEquals(StringValue);
                        }
                    }
                    else
                    {
                        if ((StringValue is not null || CharValue.HasValue) &&
                            (another.StringValue is not null || another.CharValue.HasValue))
                        {
                            return string.Equals(GetString(), another.GetString());
                        }

                        if (ByteArrayValue is not null && another.ByteArrayValue is not null)
                        {
                            return JsonValueComparer.CompareByteArray(ByteArrayValue, another.ByteArrayValue) == 0;
                        }
                    }

                    return string.Equals(GetJsonString(), another.GetJsonString());
                default:
                    throw new ArgumentOutOfRangeException(nameof(StringKind), $"Unexpected string kind {StringKind}.");
            }
        }

        public bool ValueEquals(ref JsonString another)
        {
            if (!HasElement || HasElement != another.HasElement)
            {
                return false;
            }

            return Element.ValueEquals(another.Element.GetString());
        }

        public static bool TryGetJsonString(JsonValue jsonValue, out JsonString result)
        {
            Debug.Assert(jsonValue is not null);

            // ReSharper disable once RedundantSuppressNullableWarningExpression
            if (jsonValue!.TryGetValue<JsonElement>(out var element))
            {
                if (element.ValueKind is JsonValueKind.String)
                {
                    result = new JsonString(element);
                    return true;
                }
            }
            else
            {
                if (jsonValue.TryGetValue<string>(out var stringValue))
                {
                    result = new JsonString(jsonValue, stringValue);
                    return true;
                }

                if (jsonValue.TryGetValue<DateTime>(out var dateTimeValue))
                {
                    result = new JsonString(jsonValue, dateTimeValue);
                    return true;
                }

                if (jsonValue.TryGetValue<DateTimeOffset>(out var dateTimeOffsetValue))
                {
                    result = new JsonString(jsonValue, dateTimeOffsetValue);
                    return true;
                }

                if (jsonValue.TryGetValue<Guid>(out var guidValue))
                {
                    result = new JsonString(jsonValue, guidValue);
                    return true;
                }

                if (jsonValue.TryGetValue<char>(out var charValue))
                {
                    result = new JsonString(jsonValue, charValue);
                    return true;
                }

                if (jsonValue.TryGetValue<byte[]>(out var byteArrayValue))
                {
                    result = new JsonString(jsonValue, byteArrayValue);
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
