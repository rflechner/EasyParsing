namespace EasyParsing.Dsl.Linq;

public class WhereParser<T> : ParserBase<T>
{
    private readonly IParser<T> source;
    private readonly Func<T, bool> predicate;

    public WhereParser(IParser<T> source, Func<T, bool> predicate)
    {
        this.source = source;
        this.predicate = predicate;
    }

    public override ParsingResult<T> Parse(ParsingContext context)
    {
        var result = source.Parse(context);
        if (!result.Success)
            return result;

        if (result.Result is null || !predicate(result.Result))
            return new ParsingResult<T>(result.Context, false, default, "Predicate not satisfied");

        return result;
    }
}