using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs.Formatters
{
    /// <summary>
    /// Defines methods to format <see cref="JsonDiffDelta"/> into RFC6902 Json Patch format.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6902"/>.
    /// </summary>
    public class JsonPatchDeltaFormatter : DefaultDeltaFormatter<JsonNode>
    {
        private const string PropertyNameOperation = "op";
        private const string PropertyNamePath = "path";
        private const string PropertyNameValue = "value";
        
        private const string OperationNameAdd = "add";
        private const string OperationNameRemove = "remove";
        private const string OperationNameReplace = "replace";

        public JsonPatchDeltaFormatter()
            : base(true)
        {
            PathBuilder = new();
        }

        protected StringBuilder PathBuilder { get; }

        protected override JsonNode? CreateDefault()
        {
            return new JsonArray();
        }

        protected override JsonNode? FormatArrayElement(in JsonDiffDelta.ArrayChangeEntry arrayChange,
            JsonNode? left, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, arrayChange.Index);
            return base.FormatArrayElement(arrayChange, left, existingValue);
        }

        protected override JsonNode? FormatObjectProperty(ref JsonDiffDelta delta, JsonNode? left, 
            string propertyName, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, propertyName);
            return base.FormatObjectProperty(ref delta, left, propertyName, existingValue);
        }

        protected override JsonNode? FormatAdded(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameAdd},
                {PropertyNamePath, PathBuilder.ToString()},
                {PropertyNameValue, delta.GetAdded()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatModified(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameReplace},
                {PropertyNamePath, PathBuilder.ToString()},
                {PropertyNameValue, delta.GetNewValue()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatDeleted(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameRemove},
                {PropertyNamePath, PathBuilder.ToString()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            // This should never happen. Array move operations should have been flattened into deletes and adds.
            throw new InvalidOperationException("Array move cannot be formatted.");
        }

        protected override JsonNode? FormatTextDiff(ref JsonDiffDelta delta, JsonValue? left, JsonNode? existingValue)
        {
            throw new NotSupportedException("Text diff is not supported by JsonPath.");
        }

        private readonly struct PropertyPathScope : IDisposable
        {
            private readonly StringBuilder _pathBuilder;
            private readonly int _startIndex;
            private readonly int _length;

            public PropertyPathScope(StringBuilder pathBuilder, string propertyName)
            {
                _pathBuilder = pathBuilder;
                _startIndex = pathBuilder.Length;
                pathBuilder.Append('/');
                pathBuilder.Append(Escape(propertyName));
                _length = pathBuilder.Length - _startIndex;
            }

            public PropertyPathScope(StringBuilder pathBuilder, int index)
            {
                _pathBuilder = pathBuilder;
                _startIndex = pathBuilder.Length;
                pathBuilder.Append('/');
                pathBuilder.Append(index.ToString("D"));
                _length = pathBuilder.Length - _startIndex;
            }

            public void Dispose()
            {
                _pathBuilder.Remove(_startIndex, _length);
            }

            private static string Escape(string str)
            {
                // Escape Json Pointer as per https://datatracker.ietf.org/doc/html/rfc6901#section-3
                var sb = new StringBuilder(str);
                for (var i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == '/')
                    {
                        sb.Insert(i, '~');
                        sb[++i] = '1';
                    }
                    else if (sb[i] == '~')
                    {
                        sb.Insert(i, '~');
                        sb[++i] = '0';
                    }
                }

                return sb.ToString();
            }
        }
    }
}