namespace EasyParsing;

/// <summary>
/// Represents the position of a character in a text, including its offset, column, and line.
/// </summary>
public record TextPosition(long Offset, int Column, int Line)
{
    /// <summary>
    /// Gets the zero position in the text, represented by offset 0, column 0, and line 0.
    /// </summary>
    public static TextPosition Zero => new(0, 0, 0);

    /// <summary>
    /// Increments the text position based on the given character.
    /// </summary>
    /// <param name="c">The character based on which the position is incremented.</param>
    /// <returns>A new TextPosition object with updated offset, column, and line values.</returns>
    public TextPosition IncFromChar(char c)
    {
        return this with
        {
            Offset = Offset + 1,
            Line = c == '\n' ? Line+1 : Line,
            Column = c == '\n' ? 0 : Column + 1
        };
    }

    /// <summary>
    /// Advances the text position based on the provided memory segment.
    /// </summary>
    /// <param name="m">The memory segment containing characters which will be used to update the position.</param>
    /// <returns>A new TextPosition object with updated offset, column, and line values.</returns>
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
