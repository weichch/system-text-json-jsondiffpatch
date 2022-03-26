namespace System.Text.Json.JsonDiffPatch.Diffs.Formatters
{
    public abstract class DefaultDeltaFormatter<TResult> : IJsonDiffDeltaFormatter<TResult>
    {
        public virtual TResult? Format(ref JsonDiffDelta delta)
        {
            var value = CreateDefault();
            return FormatJsonDiffDelta(ref delta, value);
        }

        protected virtual TResult? CreateDefault()
        {
            return default;
        }

        protected virtual TResult? FormatJsonDiffDelta(ref JsonDiffDelta delta, TResult? existingValue)
        {
            switch (delta.Kind)
            {
                case DeltaKind.Added:
                    existingValue = FormatAdded(ref delta, existingValue);
                    break;
                case DeltaKind.Modified:
                    existingValue = FormatModified(ref delta, existingValue);
                    break;
                case DeltaKind.Deleted:
                    existingValue = FormatDeleted(ref delta, existingValue);
                    break;
                case DeltaKind.ArrayMove:
                    existingValue = FormatArrayMove(ref delta, existingValue);
                    break;
                case DeltaKind.Text:
                    existingValue = FormatTextDiff(ref delta, existingValue);
                    break;
                case DeltaKind.Array:
                    existingValue = FormatArray(ref delta, existingValue);
                    break;
                case DeltaKind.Object:
                    existingValue = FormatObject(ref delta, existingValue);
                    break;
            }

            return existingValue;
        }

        protected abstract TResult? FormatAdded(ref JsonDiffDelta delta, TResult? existingValue);
        protected abstract TResult? FormatModified(ref JsonDiffDelta delta, TResult? existingValue);
        protected abstract TResult? FormatDeleted(ref JsonDiffDelta delta, TResult? existingValue);
        protected abstract TResult? FormatArrayMove(ref JsonDiffDelta delta, TResult? existingValue);
        protected abstract TResult? FormatTextDiff(ref JsonDiffDelta delta, TResult? existingValue);

        protected virtual TResult? FormatArray(ref JsonDiffDelta delta, TResult? existingValue)
        {
            var deltaDocument = delta.Document!.AsObject();
            foreach (var prop in deltaDocument)
            {
                if (JsonDiffDelta.IsTypeProperty(prop.Key)
                    || !JsonDiffDelta.TryGetArrayIndex(prop.Key, out var index, out var isLeft))
                {
                    continue;
                }

                var propDelta = new JsonDiffDelta(prop.Value!);
                existingValue = FormatArrayElement(ref propDelta, index, isLeft, existingValue);
            }

            return existingValue;
        }

        protected virtual TResult? FormatArrayElement(ref JsonDiffDelta delta, int index, bool isLeft, TResult? existingValue)
        {
            return FormatJsonDiffDelta(ref delta, existingValue);
        }
        
        protected virtual TResult? FormatObject(ref JsonDiffDelta delta, TResult? existingValue)
        {
            var deltaDocument = delta.Document!.AsObject();
            foreach (var prop in deltaDocument)
            {
                var propDelta = new JsonDiffDelta(prop.Value!);
                existingValue = FormatObjectProperty(ref propDelta, prop.Key, existingValue);
            }

            return existingValue;
        }

        protected virtual TResult? FormatObjectProperty(ref JsonDiffDelta delta, string propertyName, TResult? existingValue)
        {
            return FormatJsonDiffDelta(ref delta, existingValue);
        }
    }
}