namespace EasyParsing.Parsers;

/// <summary>
/// A parser that matches a specified text as a prefix of the input.
/// </summary>
/// <remarks>
/// This parser checks if the start of the remaining input matches the given text. If the input starts with the given text, the parser advances the parsing context past the matched text and returns a successful result containing the matched text; otherwise, it returns a failure result.
/// </remarks>
public class StringMatchParser(string text) : ParserBase<string>
{
    /// <inheritdoc />
    public override IParsingResult<string> Parse(ParsingContext context)
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