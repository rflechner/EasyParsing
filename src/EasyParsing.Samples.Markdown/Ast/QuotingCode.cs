namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents a code block in a Markdown abstract syntax tree (AST) that is
/// optionally associated with a specific programming language.
/// </summary>
/// <param name="Text">The code contained within the code block.</param>
/// <param name="Language">The language associated with the code block, if any.</param>
public record QuotingCode(string Text, string Language) : MarkdownAst;