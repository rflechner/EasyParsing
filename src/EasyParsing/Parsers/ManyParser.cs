namespace EasyParsing.Parsers;

/// <summary>
/// Accumulate results a parser while success.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManyParser<T> : ParserBase<T[]>
{
    private readonly IParser<T> parser;

    public ManyParser(IParser<T> parser)
    {
        this.parser = parser;
    }

    public override ParsingResult<T[]> Parse(ParsingContext context)
    {
        var results = new Queue<T>();
        var currentContext = context;
        var success = false;
        
        do
        {
            var result = parser.Parse(currentContext);
            success = result.Success;
            
            if (success)
                results.Enqueue(result.Result!);
            
            currentContext = result.Context;
        } while (success);

        if (results.Count <= 0)
            return Fail(context, "nothing matched");

        return Success(currentContext, results.ToArray());
    }
}