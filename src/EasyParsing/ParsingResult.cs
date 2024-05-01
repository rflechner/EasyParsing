namespace EasyParsing;

public record ParsingResult<T>(ParsingContext Context, bool Success, T? Result, string? FailureMessage)
{
    public static implicit operator bool(ParsingResult<T> result) => result.Success;

    public bool TryGetResult(out T result)
    {
        if (Success && Result != null)
        {
            result = Result;
            return true;
        } 

        result = default!;
        return false;
    }
}