namespace EasyParsing.Parsers;

/// <summary>
/// Parse text between 2 other matched parsings.
/// </summary>
/// <typeparam name="TLeft"></typeparam>
/// <typeparam name="TRight"></typeparam>
/// <typeparam name="T3"></typeparam>
public class BetweenParser<TLeft, T3, TRight> : ParserBase<(TLeft Before, T3 Item, TRight After)>
{
    private readonly IParser<TLeft> left;
    private readonly IParser<T3> middle;
    private readonly IParser<TRight> right;

    public BetweenParser(IParser<TLeft> left, IParser<T3> middle, IParser<TRight> right)
    {
        this.left = left;
        this.right = right;
        this.middle = middle;
    }
    
    public override ParsingResult<(TLeft Before, T3 Item, TRight After)> Parse(ParsingContext context)
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

        return Success(rightResult.Context, (leftResult.Result, middleResult.Result, rightResult.Result));
    }
}