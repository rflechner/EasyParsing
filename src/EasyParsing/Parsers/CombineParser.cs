namespace EasyParsing.Parsers;

/// <summary>
/// Combine to parsers into one new parser.
/// </summary>
/// <typeparam name="TIn1"></typeparam>
/// <typeparam name="TIn2"></typeparam>
public class CombineParser<TIn1, TIn2> : ParserBase<(TIn1, TIn2)>
{
    private readonly IParser<TIn1> left;
    private readonly IParser<TIn2> right;

    /// <summary>
    /// Combines two parsers into one new parser.
    /// </summary>
    /// <param name="left">The left parser.</param>
    /// <param name="right">The right parser.</param>
    public CombineParser(IParser<TIn1> left, IParser<TIn2> right)
    {
        this.left = left;
        this.right = right;
    }

    /// <summary>
    /// Parses the given context using the combined parsers.
    /// </summary>
    /// <param name="context">The parsing context containing the input to be parsed.</param>
    /// <returns>A parsing result that contains a tuple with the results of the two parsers if successful; otherwise, a failure message.</returns>
    public override IParsingResult<(TIn1, TIn2)> Parse(ParsingContext context)
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