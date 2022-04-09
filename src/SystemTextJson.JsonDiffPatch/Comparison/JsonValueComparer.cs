using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch.Comparison
{
    /// <summary>
    /// Contains comparer implementations for <see cref="JsonValue"/>.
    /// </summary>
    public static class JsonValueComparer
    {
        /// <summary>
        /// Returns <see cref="JsonValue"/> comparer that implements default equality. If both <see cref="JsonValue"/>
        /// instances are backed by value objects, this comparer uses <see cref="object.Equals(object)"/> method to
        /// compare the two objects. If one <see cref="JsonValue"/> instance is backed by <see cref="JsonElement"/>,
        /// this comparer deserializes the <see cref="JsonElement"/> into value object of the most significant type and
        /// compares the two value objects. If both <see cref="JsonValue" /> instances are backed by <see cref="JsonElement"/>,
        /// this comparer compares the raw value of the two <see cref="JsonElement"/> instances.
        /// </summary>
        public static IEqualityComparer<JsonValue> DefaultEquality { get; } = new DefaultJsonValueEqualityComparer();

        /// <summary>
        /// Returns <see cref="JsonValue"/> comparer that implements semantic equality. This comparer always deserializes
        /// <see cref="JsonNode"/> backed by <see cref="JsonElement"/> into value object of the most significant type
        /// and compares the value objects. This comparer is much slower than the <see cref="DefaultEquality"/> comparer
        /// but provides accurate result that is semantically correct.
        /// </summary>
        public static IEqualityComparer<JsonValue> SemanticEquality { get; } = new JsonValueSemanticEqualityComparer();
    }
}