using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using System.Threading;

namespace System.Text.Json
{
    static partial class JsonDiffPatcher
    {
        private static readonly Lazy<Func<JsonValue, object>?> ValueAccessor
            = new(CreateValueAccessor, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// This method builds a direct accessor to the <c>JsonValue&lt;object&gt;.Value</c> property.
        /// </summary>
        /// <remarks>
        /// As per benchmark result, this is the fastest way to access the value object. TryGetValue
        /// is way slower than this. This method only works for values materialized by <see cref="MaterializeJsonElement"/>.
        /// </remarks>
        private static Func<JsonValue, object>? CreateValueAccessor()
        {
            Func<JsonValue, object>? result;
            try
            {
                // This is supposed to be JsonValue<object> which in an internal type in current version
                var baseType = JsonValue.Create((object) 1)!.GetType().BaseType;
                Debug.Assert(baseType?.FullName?.StartsWith(
                    "System.Text.Json.Nodes.JsonValue`1[[System.Object") == true);
                if (baseType is null)
                {
                    return null;
                }

                var valueProperty = baseType.GetProperty("Value");
                Debug.Assert(valueProperty is not null);
                if (valueProperty is null)
                {
                    return null;
                }

                Expression<Func<JsonValue, object>> e = v => v.GetValue<object>();
                var getValueMethod = ((MethodCallExpression) e.Body).Method;

                // Build an accessor: value => (value is JsonValue<object>)
                //   ? ((JsonValue<object>)value).Value
                //   : value.GetValue<object>()
                var value = Expression.Parameter(typeof(JsonValue), "value");
                var expr = Expression.Lambda<Func<JsonValue, object>>(
                    Expression.Condition(
                        Expression.TypeIs(value, baseType),
                        Expression.Convert(Expression.MakeMemberAccess(
                                Expression.Convert(value, baseType), valueProperty),
                            typeof(object)),
                        Expression.Call(value, getValueMethod),
                        typeof(object)),
                    value);
                result = expr.Compile();
            }
            catch
            {
                result = null;
            }

            Debug.Assert(result is not null);
            return result;
        }

        internal static object? GetObjectValue(this JsonValue? value)
        {
            if (value is null)
            {
                return null;
            }

            var accessor = ValueAccessor.Value;
            if (accessor is not null)
            {
                return accessor(value);
            }

            return value.GetValue<object>();
        }
    }
}
