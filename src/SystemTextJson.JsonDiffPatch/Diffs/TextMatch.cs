namespace System.Text.Json.Diffs
{
    /// <summary>
    /// Defines a function that diffs two long texts.
    /// </summary>
    /// <param name="str1">The left string.</param>
    /// <param name="str2">The right string.</param>
    public delegate string TextMatch(string str1, string str2);
}
