namespace EasyParsing;

public abstract class ParserBase<T> : IParser<T>
{
    protected ParsingResult<T> Fail(ParsingContext context, string message)
    {
        return new ParsingResult<T>(context, false, default, message);
    }
    
    protected ParsingResult<T> Success(ParsingContext context, T result)
    {
        return new ParsingResult<T>(context, true, result, default);
    }
    
    public abstract ParsingResult<T> Parse(ParsingContext context);
    
}