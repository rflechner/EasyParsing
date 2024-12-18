namespace EasyParsing.Markdown.Ast;

/// <summary>
/// Represents a segment of code that is embedded inline within other text and typically uses quoting or
/// backticks to denote the inline code block syntax.
/// </summary>
public record InlineQuotingCode(string Text) : StyledText;