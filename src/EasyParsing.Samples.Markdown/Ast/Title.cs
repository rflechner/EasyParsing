namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents a title element in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Depth">
/// The depth level of the title, corresponding to the number of hash symbols ('#') preceding the title text in Markdown.
/// </param>
/// <param name="Content">
/// The content of the title, which is an array of MarkdownAst elements. This typically contains the text of the title.
/// </param>
public record Title(int Depth, MarkdownAst[] Content) : MarkdownAst;