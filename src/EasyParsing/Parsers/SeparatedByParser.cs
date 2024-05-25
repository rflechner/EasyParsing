namespace EasyParsing.Parsers;

public class SeparatedByParser<T, TSeparator> : ParserBase<T[]>
{
    private readonly IParser<T> itemParser;
    private readonly IParser<TSeparator> separatorParser;
    private readonly bool matchTailingSeparator;

    public SeparatedByParser(IParser<T> itemParser, IParser<TSeparator> separatorParser, bool matchTailingSeparator = false)
    {
        this.itemParser = itemParser;
        this.separatorParser = separatorParser;
        this.matchTailingSeparator = matchTailingSeparator;
    }

    public override ParsingResult<T[]> Parse(ParsingContext context)
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