using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    /// <summary>
    /// Defines a function that determines whether two items in arrays are equal.
    /// </summary>
    /// <param name="x">The element in array1.</param>
    /// <param name="indexX">The index of <paramref name="x"/> in array1.</param>
    /// <param name="y">The element in array2.</param>
    /// <param name="indexY">The index of <paramref name="y"/> in array2.</param>
    public delegate bool ArrayItemMatch(JsonNode? x, int indexX,
        JsonNode? y, int indexY, out bool deepEqual);
}
