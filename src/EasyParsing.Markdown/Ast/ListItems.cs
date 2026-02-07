namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents a collection of list items in a Markdown abstract syntax tree (AST).
/// </summary>
public record ListItems(List<ListItem> Items) : MarkdownAst;