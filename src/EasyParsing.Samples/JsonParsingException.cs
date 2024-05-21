namespace EasyParsing.Samples;

public class JsonParsingException : Exception
{
    public JsonParsingException()
    {
    }

    public JsonParsingException(string? message) : base(message)
    {
    }

    public JsonParsingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}