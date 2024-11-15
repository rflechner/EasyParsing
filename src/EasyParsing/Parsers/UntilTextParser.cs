namespace EasyParsing.Parsers;

public class UntilTextParser<T>(IParser<T> innerParser, string input, bool skipDelimiter) : ParserBase<T>
{
    public override ParsingResult<T> Parse(ParsingContext context)
    {
        var span = context.Remaining;
        var index = span.Span.IndexOf(input);
        if (index < 0)
            return Fail(context, $"{input} not found");

        var text = span[.. index];

        var subjectContext = context with
        {
            Position = TextPosition.Zero,
            Remaining = text
        };
        
        var innerResult = innerParser.Parse(subjectContext);
        if (!innerResult.Success || innerResult.Result == null)
            return Fail(context, innerResult.FailureMessage!);

        var result = innerResult.Result;

        var nextOffset = skipDelimiter ? text.Length + input.Length : text.Length;

        var nextContext = context.Forward(nextOffset);
        
        return Success(nextContext, result);
    }
}