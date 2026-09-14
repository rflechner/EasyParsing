namespace EasyParsing.Parsers;

/// <summary>
/// Tracks the range of a parser.
/// </summary>
/// <param name="parser"></param>
/// <typeparam name="T"></typeparam>
public class ParserTracker<T>(IParser<T> parser) : ParserBase<TrackedResult<T>>
{
    /// <inheritdoc />
    public override IParsingResult<TrackedResult<T>> Parse(ParsingContext context)
    {
        var start = context.Position;
        var result = parser.Parse(context);
        if (result.Success && result.Result != null)
        {
            var end = result.Context.Position;
            return Success(result.Context, new TrackedResult<T>(result.Result, new TextRange(start, end)));
        }
        
        return Fail(context, result.FailureMessage ?? "unknown error");
    }
}

/// <summary>
/// Extension methods for <see cref="ParserTracker{T}"/>.
/// </summary>
public static class TrackingExtensions
{
    /// <summary>
    /// Tracks the range of a parser.
    /// </summary>
    /// <param name="parser"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ParserTracker<T> Track<T>(this IParser<T> parser)
    {
        return new ParserTracker<T>(parser);
    }
}