namespace EasyParsing.Samples.Markdown.Ast;

/// <summary>
/// Represents an element in a Markdown abstract syntax tree (AST).
/// </summary>
public abstract record MarkdownAst;

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

/// <summary>
/// Represents an item in a Markdown list, including its depth, bullet style, content,
/// and any nested list items within it.
/// </summary>
public record ListItem(int Depth, string Bullet, MarkdownAst[] Content, Stack<ListItem> NestedList) : MarkdownAst;

/// <summary>
/// Represents a collection of list items in a Markdown abstract syntax tree (AST).
/// </summary>
public record ListItems(ListItem[] Items) : MarkdownAst;

/// <summary>
/// Represents a task list item in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Checked">Indicates whether the task is checked off.</param>
/// <param name="Content">The content of the task list item as an array of Markdown AST elements.</param>
public record TaskListItem(bool Checked, MarkdownAst[] Content) : MarkdownAst;

/// <summary>
/// Represents quoted text in a Markdown abstract syntax tree (AST).
/// </summary>
public record QuotingText(string Text) : MarkdownAst;

/// <summary>
/// Represents a code block in a Markdown abstract syntax tree (AST) that is
/// optionally associated with a specific programming language.
/// </summary>
/// <param name="Text">The code contained within the code block.</param>
/// <param name="Language">The language associated with the code block, if any.</param>
public record QuotingCode(string Text, string Language) : MarkdownAst;

/// <summary>
/// Represents the start of a paragraph in a Markdown document.
/// </summary>
public record ParagraphStart : MarkdownAst;

/// <summary>
/// Represents a carriage return line feed (CRLF) in a Markdown document.
/// </summary>
public record Crlf : MarkdownAst;

/// <summary>
/// Represents a rich text element in a Markdown abstract syntax tree (AST).
/// This is an abstract base class for various types of rich text formatting elements such as bold, italic, strikethrough, and inline code.
/// </summary>
public abstract record RichText : MarkdownAst;

/// <summary>
/// Represents a hyperlink in a Markdown document.
/// </summary>
/// <param name="Text">The anchor text of the link.</param>
/// <param name="Url">The URL that the link points to.</param>
/// <param name="Title">The optional title attribute of the link.</param>
public record Link(RichText Text, string Url, string Title) : RichText;

/// <summary>
/// Represents an image element in a Markdown abstract syntax tree (AST).
/// Contains a title and a URL.
/// </summary>
public record Image(string Title, string Url) : RichText;

/// <summary>
/// Represents bold text within Markdown content.
/// </summary>
/// <param name="Content">An array of MarkdownAst representing the content enclosed in bold markdown syntax.</param>
public record Bold(MarkdownAst[] Content) : RichText;

/// <summary>
/// Represents an italicized text element in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Content">The content that is italicized, represented as an array of <see cref="MarkdownAst"/>.</param>
public record Italic(MarkdownAst[] Content) : RichText;

/// <summary>
/// Represents text that is rendered with a strikethrough in a Markdown document.
/// </summary>
/// <param name="Content">The content of the strikethrough element, such as text or other rich text elements.</param>
public record Strikethrough(MarkdownAst[] Content) : RichText;

/// <summary>
/// Represents a segment of code that is embedded inline within other text and typically uses quoting or
/// backticks to denote the inline code block syntax.
/// </summary>
public record InlineQuotingCode(string Text) : RichText;

/// <summary>
/// Represents a raw text segment in a Markdown abstract syntax tree (AST).
/// </summary>
/// <param name="Content">The content of the raw text.</param>
public record RawText(string Content) : RichText;
