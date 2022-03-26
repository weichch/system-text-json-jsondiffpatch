using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Diffs.Formatters
{
    /// <summary>
    /// Defines methods to format <see cref="JsonDiffDelta"/> into RFC6902 Json Patch format.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6902"/>.
    /// </summary>
    public class JsonPatchDeltaFormatter : DefaultDeltaFormatter<JsonNode>
    {
        public JsonPatchDeltaFormatter()
        {
            PathBuilder = new();
        }

        protected StringBuilder PathBuilder { get; }
        
        protected override JsonNode? CreateDefault()
        {
            return new JsonArray();
        }

        protected override JsonNode? FormatArrayElement(ref JsonDiffDelta delta, int index, bool isLeft, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, index);
            return base.FormatArrayElement(ref delta, index, isLeft, existingValue);
        }

        protected override JsonNode? FormatObjectProperty(ref JsonDiffDelta delta, string propertyName, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, propertyName);
            return base.FormatObjectProperty(ref delta, propertyName, existingValue);
        }

        protected override JsonNode? FormatAdded(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {"op", "add"},
                {"path", PathBuilder.ToString()},
                {"value", delta.GetAdded()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatModified(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {"op", "replace"},
                {"path", PathBuilder.ToString()},
                {"value", delta.GetNewValue()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatDeleted(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {"op", "remove"},
                {"path", PathBuilder.ToString()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            throw new NotImplementedException();
        }

        protected override JsonNode? FormatTextDiff(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            throw new NotImplementedException();
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
                pathBuilder.Append(propertyName);
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
        }
    }
}