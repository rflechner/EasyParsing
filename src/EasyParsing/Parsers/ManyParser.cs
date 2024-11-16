namespace EasyParsing.Parsers;

/// <summary>
/// Accumulate results a parser while success.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManyParser<T> : ParserBase<IEnumerable<T>>
{
    private readonly IParser<T> parser;

    /// <summary>
    /// Accumulates the results of a specified parser as long as it successfully parses the input.
    /// </summary>
    /// <param name="parser">The parser to accumulate results.</param>
    public ManyParser(IParser<T> parser)
    {
        this.parser = parser;
    }

    /// <inheritdoc />
    public override IParsingResult<IEnumerable<T>> Parse(ParsingContext context)
    {
        var results = new Queue<T>();
        var currentContext = context;
        bool success;
        
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