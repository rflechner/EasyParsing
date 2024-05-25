namespace EasyParsing.Parsers;

/// <summary>
/// Match a text.
/// </summary>
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