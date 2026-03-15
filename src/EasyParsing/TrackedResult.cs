namespace EasyParsing;

/// <summary>
/// Tracks the range of a parser.
/// </summary>
/// <param name="Value"></param>
/// <param name="Range"></param>
/// <typeparam name="T"></typeparam>
public record TrackedResult<T>(T Value, TextRange Range)
{
    /// <summary>
    /// Combines two tracked results, taking the start of A and the end of B.
    /// </summary>
    public static TrackedResult<T> operator +(TrackedResult<T> a, TrackedResult<T> b)
    {
        return a with { Range = new TextRange(a.Range.Start, b.Range.End) };
    }
}
