namespace EasyParsing.Samples.Markdown;

public abstract record MarkdownAst;

public record Title(int Depth, string Text) : MarkdownAst;

public record HyperLink(string Text, string Url) : MarkdownAst;