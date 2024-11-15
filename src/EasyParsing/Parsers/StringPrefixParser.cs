namespace EasyParsing.Parsers;

public class StringPrefixParser(string text) : ParserBase<string>
{
    public override ParsingResult<string> Parse(ParsingContext context)
    {
        if (context.Remaining.Length < text.Length)
            return Fail(context, "End of text");
        
        var slice = context.Remaining[..text.Length];
        var target = slice.ToString();
        if (text.Equals(target))
            return Success(context.ForwardMemory(slice), text);
        
        return Fail(context, $"'{text}' not matching '{target}'");
    }
}