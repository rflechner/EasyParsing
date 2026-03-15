namespace EasyParsing;

/// <summary>
/// Represents a range of text within a document.
/// </summary>
/// <param name="Start">Start position of the text range.</param>
/// <param name="End">End position of the text range.</param>
public record TextRange(TextPosition Start, TextPosition End)
{
    /// <summary>
    /// Combines two tracked results, taking the start of A and the end of B.
    /// </summary>
    public static TextRange operator +(TextRange a, TextRange b)
    {
        return a with { End = b.End };
    }
}