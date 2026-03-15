namespace EasyParsing;

/// <summary>
/// Represents a range of text within a document.
/// </summary>
/// <param name="Start">Start position of the text range.</param>
/// <param name="End">End position of the text range.</param>
public record TextRange(TextPosition Start, TextPosition End);