namespace EasyParsing.Parsers;

public class UntilParser<T1, T2>(IParser<T1> innerParser, IParser<T2> limitParser) : ParserBase<(T1, T2)>
{
    public override ParsingResult<(T1, T2)> Parse(ParsingContext context)
    {
        var currentContext = context;

        var parsingResult = limitParser.Parse(currentContext);
        while (!parsingResult && !currentContext.Remaining.IsEmpty)
        {
            currentContext = currentContext.Forward(1);
            parsingResult = limitParser.Parse(currentContext);
        }

        if (currentContext.Remaining.IsEmpty)
            return Fail(context, "limit parser not satisfied");

        var subjectContext = context with
        {
            Position = TextPosition.Zero,
            Remaining = context.Remaining[..(int)currentContext.Position.Offset]
        };
        
        var innerResult = innerParser.Parse(subjectContext);
        if (!innerResult.Success || innerResult.Result == null)
            return Fail(context, innerResult.FailureMessage!);

        (T1, T2) result = (innerResult.Result, parsingResult.Result!);

        return Success(parsingResult.Context, result);
    }
}