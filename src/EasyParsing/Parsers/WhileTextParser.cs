namespace EasyParsing.Parsers;

/// <summary>
/// A parser that consumes input characters as long as a specified condition is met.
/// Inherits from the ParserBase class to provide common parsing functionalities.
/// </summary>
public class WhileTextParser : ParserBase<string>
{
    private readonly Func<ReadOnlyMemory<char>, bool> condition;

    /// <summary>
    /// A parser that consumes input characters as long as a specified condition is met.
    /// Inherits from the ParserBase class to provide common parsing functionalities.
    /// </summary>
    /// <param name="condition">The condition to satisfy.</param>
    public WhileTextParser(Func<ReadOnlyMemory<char>, bool> condition)
    {
        this.condition = condition;
    }

    /// <inheritdoc />
    public override IParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining;

        for (var i = 0; i <= span.Length; i++)
        {
            if (!condition(span[..i]))
            {
                if (i <= 0)
                    return Fail(context, "Nothing matched");
                return Success(context.Forward(i-1), span[..(i-1)].ToString());
            }
        }
        
        // if (span.IsEmpty) 
            return Fail(context, "Nothing matched");
        
        //return Success(context.Forward(span.Length), span.ToString());
    }
}