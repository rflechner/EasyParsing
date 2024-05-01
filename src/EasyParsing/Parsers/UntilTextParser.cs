namespace EasyParsing.Parsers;

public class UntilTextParser : ParserBase<string>
{
    private readonly string input;
    private readonly bool skipMatch;

    public UntilTextParser(string input, bool skipMatch = true)
    {
        this.input = input;
        this.skipMatch = skipMatch;
    }

    public override ParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining;
        var index = span.Span.IndexOf(input);
        if (index < 0)
            return Fail(context, $"{input} not found");

        var text = span[.. index];
        ReadOnlyMemory<char> rest;
        if (skipMatch)
        {
            rest = span[(text.Length + input.Length) ..];
            return Success(new ParsingContext(rest, context.Position.ForwardMemory(span[.. (index-1 + text.Length-1)])), text.ToString());
        }

        rest = span[text.Length ..];
        
        return Success(new ParsingContext(rest, context.Position.ForwardMemory(text)), text.ToString());
    }
}