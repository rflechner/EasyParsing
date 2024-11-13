namespace EasyParsing.Parsers;

public class OptionnalParser<T>(IParser<T> innerParser) : IParser<Option<T>>
{
    public ParsingResult<Option<T>> Parse(ParsingContext context)
    {
        var result = innerParser.Parse(context);

        if (result.Success)
            return new ParsingResult<Option<T>>(result.Context, true, new Some<T>(result.Result!), result.FailureMessage);

        return new ParsingResult<Option<T>>(result.Context, true, new None<T>(), result.FailureMessage);
    }
}