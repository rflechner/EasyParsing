namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents a collection of list items in a Markdown abstract syntax tree (AST).
/// </summary>
public record ListItems(ListItem[] Items) : MarkdownAst;