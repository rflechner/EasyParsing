namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents bold text within Markdown content.
/// </summary>
/// <param name="Content">An array of MarkdownAst representing the content enclosed in bold markdown syntax.</param>
public record Bold(MarkdownAst[] Content) : StyledText;