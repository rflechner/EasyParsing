namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents a task list item in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Checked">Indicates whether the task is checked off.</param>
/// <param name="Content">The content of the task list item as an array of Markdown AST elements.</param>
public record TaskListItem(bool Checked, MarkdownAst[] Content) : MarkdownAst;