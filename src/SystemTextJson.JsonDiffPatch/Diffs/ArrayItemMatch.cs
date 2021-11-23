namespace System.Text.Json.JsonDiffPatch.Diffs
{
    /// <summary>
    /// Defines a function that determines whether two items in arrays are equal.
    /// </summary>
    /// <param name="context">The comparison context.</param>
    public delegate bool ArrayItemMatch(ref ArrayItemMatchContext context);
}
