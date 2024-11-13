namespace EasyParsing.Samples.Markdown;

public abstract record MarkdownAst;

public record Title(int Depth, string Text) : MarkdownAst;

public record Link(string Text, string Url, string Title) : MarkdownAst;

public record Image(string Text, string Url, string Title) : MarkdownAst;
