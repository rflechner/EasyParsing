namespace EasyParsing.Parsers;

/// <summary>
/// Match a char satisfying a condition.
/// </summary>
public class SatisfyParser : ParserBase<char>
{
    private readonly Func<char, bool> condition;

    /// <summary>
    /// Parses a single character that satisfies a given condition.
    /// </summary>
    /// <param name="condition">The condition to satisfy.</param>
    public SatisfyParser(Func<char, bool> condition)
    {
        this.condition = condition;
    }
    
    /// <inheritdoc />
    public override IParsingResult<char> Parse(ParsingContext context)
    {
        if (context.Remaining.Length <= 0)
            return Fail(context, "End of stream");
        
        var c = context.Remaining.Span[0];
        if (condition(c))
            return Success(context.ForwardChar(c), c);
        
        return Fail(context, $"{c}' does not satisfy condition");
    }
}