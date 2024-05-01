namespace EasyParsing.Parsers;

public class LookupStringParser : ParserBase<string>
{
    private readonly string searchedText;

    public LookupStringParser(string searchedText)
    {
        this.searchedText = searchedText;
    }

    public override ParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining.Span;
        if (span.Length < Math.Max(1, searchedText.Length))
            return Fail(context, "end of buffer");

        if (!span[.. searchedText.Length].SequenceEqual(searchedText))
            return Fail(context, "not expected char");

        return Success(context.ForwardMemory(searchedText.AsMemory()), searchedText);
        
    }
}

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