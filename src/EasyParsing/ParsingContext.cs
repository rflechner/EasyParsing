namespace EasyParsing;

public record ParsingContext(ReadOnlyMemory<char> Remaining, TextPosition Position)
{
    public static ParsingContext FromString(string text) => new(text.AsMemory(), TextPosition.Zero);

    public ParsingContext Forward(int count) =>
        this with
        {
            Remaining = Remaining[count..], 
            Position = Position.ForwardMemory(Remaining[..count])
        };

    public ParsingContext ForwardChar(char c) =>
        this with
        {
            Remaining = Remaining[1..], 
            Position = Position.IncFromChar(c)
        };

    public ParsingContext ForwardMemory(ReadOnlyMemory<char> memory) =>
        this with
        {
            Remaining = Remaining[memory.Length..], 
            Position = Position.ForwardMemory(memory)
        };

}