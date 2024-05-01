namespace EasyParsing.Parsers;

public class BetweenParser<TLeft, TRight, T3> : ParserBase<(TLeft, TRight, T3)>
{
    private readonly IParser<TLeft> left;
    private readonly IParser<TRight> right;
    private readonly IParser<T3> middle;

    public BetweenParser(IParser<TLeft> left, IParser<T3> middle, IParser<TRight> right)
    {
        this.left = left;
        this.right = right;
        this.middle = middle;
    }
    
    public override ParsingResult<(TLeft, TRight, T3)> Parse(ParsingContext context)
    {
        var leftResult = left.Parse(context);
        if (!leftResult.Success || leftResult.Result == null)
            return Fail(context, leftResult.FailureMessage!);

        var middleResult = middle.Parse(leftResult.Context);
        if (!middleResult.Success || middleResult.Result == null)
            return Fail(context, middleResult.FailureMessage!);
        
        var rightResult = right.Parse(middleResult.Context);
        if (!rightResult.Success || rightResult.Result == null)
            return Fail(context, rightResult.FailureMessage!);

        return Success(rightResult.Context, (leftResult.Result, rightResult.Result, middleResult.Result));
    }
}