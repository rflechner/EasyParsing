namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents quoted text in a Markdown abstract syntax tree (AST).
/// </summary>
public record QuotingText(string Text) : MarkdownAst;