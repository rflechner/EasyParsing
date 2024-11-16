namespace EasyParsing;

/// <summary>
/// Represents the result of a parsing operation. Provides information about the
/// parsing context in which the parsing occurred, the success status of the operation,
/// the parsed result if successful, and an optional failure message if the parsing failed.
/// </summary>
/// <typeparam name="T">The type of the parsed result.</typeparam>
public interface IParsingResult<out T>
{
    /// <summary>
    /// Gets the current parsing context.
    /// The context provides details about the remaining input and the current position
    /// in the parsed text. It is utilized by parsers to track the progress of parsing
    /// operations and ensure correct parsing behavior.
    /// </summary>
    ParsingContext Context { get; }

    /// <summary>
    /// Indicates whether the parsing operation was successful.
    /// Returns true if the parsing was completed without errors
    /// or false if the parsing encountered errors or did not meet criteria.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets the result of the parsing operation.
    /// This property holds the parsed value if the parsing was successful;
    /// otherwise, it is null. It provides access to the outcome of the parsing
    /// process, allowing further use or validation of the parsed data.
    /// </summary>
    T? Result { get; }

    /// <summary>
    /// Gets an optional message describing the reason for failure if the parsing
    /// operation was unsuccessful. This message provides additional context or
    /// details about why the parsing did not succeed.
    /// </summary>
    string? FailureMessage { get; }
}