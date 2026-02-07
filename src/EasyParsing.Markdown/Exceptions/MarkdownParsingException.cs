namespace EasyParsing.Markdown.Exceptions;

public class MarkdownParsingException : Exception
{
    public MarkdownParsingException(string? message) : base(message)
    {
    }

    public MarkdownParsingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}