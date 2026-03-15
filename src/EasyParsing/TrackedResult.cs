namespace EasyParsing;

/// <summary>
/// Tracks the range of a parser.
/// </summary>
/// <param name="Value"></param>
/// <param name="Range"></param>
/// <typeparam name="T"></typeparam>
public record TrackedResult<T>(T Value, TextRange Range);
