namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents text that is rendered with a strikethrough in a Markdown document.
/// </summary>
/// <param name="Content">The content of the strikethrough element, such as text or other rich text elements.</param>
public record Strikethrough(MarkdownAst[] Content) : StyledText;