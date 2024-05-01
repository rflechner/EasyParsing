namespace EasyParsing;

public record TextPosition(long Offset, int Column, int Line)
{
    public static TextPosition Zero => new(0, 0, 0);

    public TextPosition IncFromChar(char c)
    {
        return this with
        {
            Offset = Offset + 1,
            Line = c == '\n' ? Line+1 : Line,
            Column = c == '\n' ? 0 : Column + 1
        };
    }
    
    public TextPosition ForwardMemory(ReadOnlyMemory<char> m)
    {
        var line = Line;
        var column = Column;
        var span = m.Span;

        for (var i = 0; i < m.Length; i++)
        {
            var c = span[i];
            if (c == '\n')
            {
                line ++;
                column = 0;
                continue;
            }

            column++;
        }

        return this with
        {
            Offset = Offset + m.Length,
            Line = line,
            Column = column
        };
    }
    
}
