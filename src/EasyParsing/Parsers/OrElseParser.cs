namespace EasyParsing.Parsers;

public class OrElseParser<T> : ParserBase<T>
{
    private readonly IParser<T>[] parsers;

    public OrElseParser(IParser<T>[] parsers)
    {
        this.parsers = parsers;
    }
    
    public override ParsingResult<T> Parse(ParsingContext context)
    {
        var failureMessages = new List<string>();
        
        foreach (var parser in parsers)
        {
            var result = parser.Parse(context);
            if (result.Success && result.Result != null)
                return Success(context, result.Result);
            
            if (result.FailureMessage != null)
                failureMessages.Add(result.FailureMessage);
        }

        return Fail(context, string.Join(" AND ", failureMessages));
    }
}