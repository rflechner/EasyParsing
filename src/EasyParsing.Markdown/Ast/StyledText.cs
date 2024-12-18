namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents a rich text element in a Markdown abstract syntax tree (AST).
/// This is an abstract base class for various types of rich text formatting elements such as bold, italic, strikethrough, and inline code.
/// </summary>
public abstract record StyledText : MarkdownAst;