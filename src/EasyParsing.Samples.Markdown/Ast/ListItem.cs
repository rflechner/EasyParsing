namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents an item in a Markdown list, including its depth, bullet style, content,
/// and any nested list items within it.
/// </summary>
public record ListItem(int Depth, string Bullet, MarkdownAst[] Content, Stack<ListItem> NestedList) : MarkdownAst;