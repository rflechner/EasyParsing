namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents a hyperlink in a Markdown document.
/// </summary>
/// <param name="Text">The anchor text of the link.</param>
/// <param name="Url">The URL that the link points to.</param>
/// <param name="Title">The optional title attribute of the link.</param>
public record Link(StyledText Text, string Url, string Title) : StyledText;