namespace EasyParsing.Parsers;

/// <summary>
/// Parses a sequence of items separated by a specified separator.
/// </summary>
/// <typeparam name="T">The type of the items to be parsed.</typeparam>
/// <typeparam name="TSeparator">The type of the separator to be used.</typeparam>
public class SeparatedByParser<T, TSeparator> : ParserBase<T[]>
{
    private readonly IParser<T> itemParser;
    private readonly IParser<TSeparator> separatorParser;
    private readonly bool matchTailingSeparator;

    /// <summary>
    /// Create parser of items separated by a specified separator.
    /// </summary>
    /// <param name="itemParser">Items parser</param>
    /// <param name="separatorParser">Separator parser</param>
    /// <param name="matchTailingSeparator">Defines if trailing separator must be consumed</param>
    public SeparatedByParser(IParser<T> itemParser, IParser<TSeparator> separatorParser, bool matchTailingSeparator = false)
    {
        this.itemParser = itemParser;
        this.separatorParser = separatorParser;
        this.matchTailingSeparator = matchTailingSeparator;
    }

    /// <inheritdoc />
    public override IParsingResult<T[]> Parse(ParsingContext context)
    {
        var queue = new Queue<T>();

        var currentContext = context;
        var itemResult = itemParser.Parse(currentContext);

        do
        {
            if (!itemResult.Success || itemResult.Result == null)
            {
                if (queue.Count <= 0)
                    return Fail(currentContext, itemResult.FailureMessage!);
                return Success(itemResult.Context, queue.ToArray());
            }

            var separatorResult = separatorParser.Parse(itemResult.Context);
            if (!separatorResult.Success || separatorResult.Result == null)
            {
                if (queue.Count <= 0)
                    return Fail(currentContext, separatorResult.FailureMessage!);

                queue.Enqueue(itemResult.Result);

                return Success(separatorResult.Context, queue.ToArray());
            }

            queue.Enqueue(itemResult.Result);

            var previousContext = itemResult.Context;
            itemResult = itemParser.Parse(separatorResult.Context);
            
            if (!matchTailingSeparator && !itemResult.Success)
            {
                if (queue.Count <= 0)
                    return Fail(currentContext, itemResult.FailureMessage!);
                return Success(previousContext, queue.ToArray());
            }

            currentContext = separatorResult.Context;
            
        } while (!currentContext.Remaining.IsEmpty);
        
        return Success(currentContext, queue.ToArray());
    }
}