namespace EasyParsing.Parsers;

public class WhileTextParser : ParserBase<string>
{
    private readonly Func<ReadOnlyMemory<char>, bool> condition;

    public WhileTextParser(Func<ReadOnlyMemory<char>, bool> condition)
    {
        this.condition = condition;
    }

    public override ParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining;

        for (var i = 0; i <= span.Length; i++)
        {
            var current = span[..i];
            if (!condition(current))
            {
                if (i <= 0)
                    return Fail(context, "Nothing matched");
                return Success(context.Forward(i), current.ToString());
            }
        }
        
        // if (span.IsEmpty) 
            return Fail(context, "Nothing matched");
        
        //return Success(context.Forward(span.Length), span.ToString());
    }
}