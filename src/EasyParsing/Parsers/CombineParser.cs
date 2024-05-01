namespace EasyParsing.Parsers;

public class CombineParser<TIn1, TIn2> : ParserBase<(TIn1, TIn2)>
{
    private readonly IParser<TIn1> left;
    private readonly IParser<TIn2> right;

    public CombineParser(IParser<TIn1> left, IParser<TIn2> right)
    {
        this.left = left;
        this.right = right;
    }

    public override ParsingResult<(TIn1, TIn2)> Parse(ParsingContext context)
    {
        var result1 = left.Parse(context);
        if (!result1.Success || result1.Result == null)
            return Fail(context, result1.FailureMessage!);
        
        var result2 = right.Parse(result1.Context);
        if (!result2.Success || result2.Result == null)
            return Fail(context, result2.FailureMessage!);

        return Success(result2.Context, (result1.Result, result2.Result));
    }
}