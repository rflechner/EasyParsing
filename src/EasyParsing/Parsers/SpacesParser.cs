namespace EasyParsing.Parsers;

/// <summary>
/// Parses consecutive whitespace characters from the input string.
/// </summary>
/// <remarks>
/// SpacesParser identifies sequences of whitespace characters (spaces, tabs, etc.) and can be configured to fail if no spaces are found.
/// </remarks>
public class SpacesParser(bool failIfNothingMatched) : ParserBase<string>
{
    /// <inheritdoc />
    public override IParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining.Span;
        var index = 0;

        while (index < span.Length)
        {
            if (!char.IsWhiteSpace(span[index]))
                break;
            index++;
        }

        if (index == 0)
        {
            return failIfNothingMatched 
                ? Fail(context, "No spaces matched") 
                : Success(context, string.Empty);
        }

        var spaces = span[.. index];

        return Success(context.Forward(index), spaces.ToString());
    }
}