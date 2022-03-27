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
        private const string PropertyNameFrom = "from";
        private const string OperationNameAdd = "add";
        private const string OperationNameRemove = "remove";
        private const string OperationNameReplace = "replace";
        private const string OperationNameMove = "move";

        public JsonPatchDeltaFormatter()
        {
            PathBuilder = new();
        }

        protected StringBuilder PathBuilder { get; }

        protected override JsonNode? CreateDefault()
        {
            return new JsonArray();
        }

        protected override JsonNode? FormatArrayElement(ref JsonDiffDelta delta, int index, bool isLeft,
            JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, index);
            return base.FormatArrayElement(ref delta, index, isLeft, existingValue);
        }

        protected override JsonNode? FormatObjectProperty(ref JsonDiffDelta delta, string propertyName,
            JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, propertyName);
            return base.FormatObjectProperty(ref delta, propertyName, existingValue);
        }

        protected override JsonNode? FormatAdded(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var arr = existingValue!.AsArray();
            var path = PathBuilder.ToString();
            if (arr.Count > 0)
            {
                // If the last operation is remove at the same path, simply merge the two operations with a replace
                var lastOp = arr[arr.Count - 1]!.AsObject();
                if (string.Equals(lastOp[PropertyNameOperation]!.GetValue<string>(), OperationNameRemove)
                    && string.Equals(lastOp[PropertyNamePath]!.GetValue<string>(), path))
                {
                    arr.RemoveAt(arr.Count - 1);
                    var modifiedDelta = new JsonDiffDelta();
                    modifiedDelta.Modified("", delta.GetAdded());
                    return FormatModified(ref modifiedDelta, existingValue);
                }
            }

            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameAdd},
                {PropertyNamePath, PathBuilder.ToString()},
                {PropertyNameValue, delta.GetAdded()}
            };
            arr.Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatModified(ref JsonDiffDelta delta, JsonNode? existingValue)
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

        protected override JsonNode? FormatDeleted(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameRemove},
                {PropertyNamePath, PathBuilder.ToString()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameMove},
                {PropertyNameFrom, PathBuilder.ToString()},
                {PropertyNamePath, GetMoveTargetPath(delta.GetNewIndex())}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        protected override JsonNode? FormatTextDiff(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            throw new NotSupportedException("Text diff is not supported by JsonPath.");
        }

        private string GetMoveTargetPath(int newIndex)
        {
            var targetPathBuilder = new StringBuilder(PathBuilder.ToString());

            while (targetPathBuilder.Length > 0 && targetPathBuilder[targetPathBuilder.Length - 1] != '/')
            {
                targetPathBuilder.Remove(targetPathBuilder.Length - 1, 1);
            }

            if (targetPathBuilder.Length == 0)
            {
                targetPathBuilder.Append('/');
            }

            targetPathBuilder.Append(newIndex.ToString("D"));
            return targetPathBuilder.ToString();
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