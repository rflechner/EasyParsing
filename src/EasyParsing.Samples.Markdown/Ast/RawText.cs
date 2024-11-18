namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents a raw text segment in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Content">The content of the raw text.</param>
public record RawText(string Content) : StyledText;