namespace EasyParsing.Samples.Markdown;

public abstract record MarkdownAst;

public record Title(int Depth, string Text) : MarkdownAst;
public record Paragraph(RichText Content) : MarkdownAst;

public abstract record RichText : MarkdownAst;

public record Link(RichText Text, string Url, string Title) : RichText;

public record Image(string Title, string Url) : RichText;

public record Bold(RichText Text) : RichText;

public record Italic(RichText Text) : RichText;

public record RawText(string Content) : RichText;
