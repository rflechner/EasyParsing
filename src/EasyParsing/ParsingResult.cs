namespace EasyParsing;

/// <summary>
/// Represents the result of a parsing operation, including the context in which the parsing occurred,
/// the success status, the parsed result (if successful), and an optional failure message.
/// </summary>
/// <typeparam name="T">The type of the parsed result.</typeparam>
public record ParsingResult<T>(ParsingContext Context, bool Success, T? Result, string? FailureMessage) : IParsingResult<T>
{
    /// <summary>
    /// Implements an implicit operator for converting a ParsingResult of type T to a boolean value.
    /// Returns the Success property, indicating whether the parsing was successful.
    /// </summary>
    /// <param name="result">The ParsingResult instance to be converted to a boolean.</param>
    /// <returns>True if the parsing was successful, otherwise false.</returns>
    public static implicit operator bool(ParsingResult<T> result) => result.Success;

    /// <summary>
    /// Attempts to retrieve the parsed result if the parsing was successful.
    /// </summary>
    /// <param name="result">When this method returns, contains the parsed result if the parsing was successful; otherwise, the default value for the type.</param>
    /// <returns>True if the parsing was successful and the result is available; otherwise, false.</returns>
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