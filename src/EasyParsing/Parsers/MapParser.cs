namespace EasyParsing.Parsers;

/// <summary>
/// Transform a parser result.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class MapParser<TIn, TOut> : ParserBase<TOut>
{
    private readonly IParser<TIn> input;
    private readonly Func<TIn, TOut> map;

    public MapParser(IParser<TIn> input, Func<TIn, TOut> map)
    {
        this.input = input;
        this.map = map;
    }

    public override ParsingResult<TOut> Parse(ParsingContext context)
    {
        var inputResult = input.Parse(context);
        if (!inputResult.Success || inputResult.Result == null)
            return Fail(context, inputResult.FailureMessage!);

        var output = map(inputResult.Result);
        
        return Success(inputResult.Context, output);
    }
}