namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents an italicized text element in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Content">The content that is italicized, represented as an array of <see cref="MarkdownAst"/>.</param>
public record Italic(MarkdownAst[] Content) : StyledText;