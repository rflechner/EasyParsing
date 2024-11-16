namespace EasyParsing.Parsers;

/// <summary>
/// A parser that attempts to parse the input using multiple parsers in sequence until one succeeds.
/// </summary>
/// <typeparam name="T">The type of the result produced by the parser.</typeparam>
public class OrElseParser<T> : ParserBase<T>
{
    private readonly IParser<T>[] parsers;

    /// <summary>
    /// A parser that attempts to parse the input using multiple parsers in sequence until one succeeds.
    /// </summary>
    /// <param name="parsers">The parsers to try.</param>
    public OrElseParser(IParser<T>[] parsers)
    {
        this.parsers = parsers;
    }
    
    /// <inheritdoc />
    public override IParsingResult<T> Parse(ParsingContext context)
    {
        var failureMessages = new List<string>();
        
        foreach (var parser in parsers)
        {
            var result = parser.Parse(context);
            if (result.Success && result.Result != null)
                return Success(result.Context, result.Result);
            
            if (result.FailureMessage != null)
                failureMessages.Add(result.FailureMessage);
        }

        return Fail(context, string.Join(" AND ", failureMessages));
    }
}