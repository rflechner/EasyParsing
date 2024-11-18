namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents an image element in a Markdown abstract syntax tree (AST).
/// Contains a title and a URL.
/// </summary>
public record Image(string Title, string Url) : StyledText;