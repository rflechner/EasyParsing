namespace EasyParsing;

/// <summary>
/// Represents the context for parsing operations, encapsulating the remaining text to be parsed and the current text position.
/// </summary>
/// <param name="Remaining">
/// The portion of text that is yet to be parsed, represented as a ReadOnlyMemory of characters.
/// </param>
/// <param name="Position">
/// The current position in the text, including information about offset, column, and line.
/// </param>
public record ParsingContext(ReadOnlyMemory<char> Remaining, TextPosition Position)
{
    /// <summary>
    /// Creates a new instance of <see cref="ParsingContext"/> from the given string.
    /// </summary>
    /// <param name="text">The input string to initialize the parsing context.</param>
    /// <returns>A new instance of <see cref="ParsingContext"/> initialized with the input string and starting at position zero.</returns>
    public static ParsingContext FromString(string text) => new(text.AsMemory(), TextPosition.Zero);

    /// <summary>
    /// Defines an implicit conversion from a string to a <see cref="ParsingContext"/>.
    /// </summary>
    /// <param name="text">The input string to be converted into a <see cref="ParsingContext"/>.</param>
    /// <returns>A new <see cref="ParsingContext"/> initialized with the input string and starting at position zero.</returns>
    public static implicit operator ParsingContext(string text) => FromString(text);

    /// <summary>
    /// Advances the current <see cref="ParsingContext"/> by the specified character count.
    /// </summary>
    /// <param name="count">The number of characters to advance.</param>
    /// <returns>A new instance of <see cref="ParsingContext"/> with the position moved forward by the specified count.</returns>
    public ParsingContext Forward(int count) =>
        this with
        {
            Remaining = Remaining[count..], 
            Position = Position.ForwardMemory(Remaining[..count])
        };

    /// <summary>
    /// Advances the parsing context by moving forward one character and updating the text position accordingly.
    /// </summary>
    /// <param name="c">The character to process while moving forward.</param>
    /// <returns>A new instance of <see cref="ParsingContext"/> with the updated position and remaining text.</returns>
    public ParsingContext ForwardChar(char c) =>
        this with
        {
            Remaining = Remaining[1..], 
            Position = Position.IncFromChar(c)
        };

    /// <summary>
    /// Advances the parsing context by the specified memory segment.
    /// </summary>
    /// <param name="memory">The memory segment to advance by.</param>
    /// <returns>A new instance of <see cref="ParsingContext"/> with the text and the position advanced by the specified memory segment.</returns>
    public ParsingContext ForwardMemory(ReadOnlyMemory<char> memory) =>
        this with
        {
            Remaining = Remaining[memory.Length..], 
            Position = Position.ForwardMemory(memory)
        };

}