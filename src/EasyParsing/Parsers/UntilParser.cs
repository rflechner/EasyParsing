namespace EasyParsing.Parsers;

/// <summary>
/// Represents a parser that processes input using an inner parser until a limit parser is matched.
/// </summary>
/// <typeparam name="T1">The type of the result produced by the inner parser.</typeparam>
/// <typeparam name="T2">The type of the result produced by the limit parser.</typeparam>
public class UntilParser<T1, T2>(IParser<T1> innerParser, IParser<T2> limitParser) : ParserBase<(T1, T2)>
{
    /// <inheritdoc />
    public override IParsingResult<(T1, T2)> Parse(ParsingContext context)
    {
        var currentContext = context;

        var parsingResult = limitParser.Parse(currentContext);
        while (!parsingResult.Success && !currentContext.Remaining.IsEmpty)
        {
            currentContext = currentContext.Forward(1);
            parsingResult = limitParser.Parse(currentContext);
        }

        if (currentContext.Remaining.IsEmpty)
            return Fail(context, "limit parser not satisfied");

        var subjectContext = context with
        {
            Position = TextPosition.Zero,
            Remaining = context.Remaining[..(int)(currentContext.Position.Offset - context.Position.Offset)]
        };
        
        var innerResult = innerParser.Parse(subjectContext);
        if (!innerResult.Success || innerResult.Result == null)
            return Fail(context, innerResult.FailureMessage!);

        (T1, T2) result = (innerResult.Result, parsingResult.Result!);

        return Success(parsingResult.Context, result);
    }
}