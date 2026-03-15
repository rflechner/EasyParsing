using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;

namespace EasyParsing.Dsl;

/// <summary>
/// Provides a set of static methods for parsing various types of text inputs.
/// </summary>
public static class Parse
{
    /// <summary>
    /// Determines if the given character is a newline character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <return>True if the character is '\r' or '\n'; otherwise, false.</return>
    public static bool IsNewLine(char c) => c is '\r' or '\n';
    
    /// <summary>
    /// Creates a parser that matches the specified text as a prefix.
    /// </summary>
    /// <param name="text">The text to match as a prefix.</param>
    /// <return>An <see cref="IParser{T}"/> that matches the specified text as a prefix.</return>
    public static IParser<string> StringMatch(string text) => new StringMatchParser(text);

    /// <summary>
    /// Parses a sequence of characters representing an integer and converts it into an integer type.
    /// </summary>
    /// <return>An IParser instance that parses a sequence of digits and converts it to an integer.</return>
    public static IParser<int> Integer() => ManySatisfy(char.IsDigit).Select(int.Parse);

    /// <summary>
    /// Converts the given parser into an optional parser.
    /// </summary>
    /// <param name="parser">The parser to be converted.</param>
    /// <return>An optional parser that will attempt to parse the input using the given parser, returning an option type.</return>
    public static OptionnalParser<T> Optionnal<T>(this IParser<T> parser) => new OptionnalParser<T>(parser);

    /// <summary>
    /// Creates a parser that skips over any whitespace characters.
    /// </summary>
    /// <return>An <see cref="IParser{T}"/> that skips over spaces.</return>
    public static IParser<string> SkipSpaces() => new SpacesParser(failIfNothingMatched: false);

    /// <summary>
    /// Parses a sequence of whitespace characters, including spaces and tabs,
    /// and returns the matched sequence.
    /// Fails if nothing is matched.
    /// </summary>
    /// <return>An IParser instance that parses whitespace characters.</return>
    public static IParser<string> Spaces() => new SpacesParser(failIfNothingMatched: true);

    /// <summary>
    /// Parses and matches inline space characters such as spaces and tabs.
    /// </summary>
    /// <return>An IParser instance that matches any space or tab character.</return>
    public static IParser<string> InlineSpaces() => AnyOf(' ', '\t');

    /// <summary>
    /// Creates a parser that matches one or more newline characters.
    /// </summary>
    /// <return>A parser that matches newline characters ('\r' or '\n').</return>
    public static IParser<string> NewLine() => ManySatisfy(IsNewLine);

    /// <summary>
    /// Creates a parser that matches a single specified character.
    /// </summary>
    /// <param name="c">The character to be matched.</param>
    /// <returns>An <see cref="IParser{T}"/> that matches the specified character.</returns>
    public static IParser<char> OneChar(char c) => new OneCharParser(c);

    /// <summary>
    /// Creates a parser that parses a specific character and converts it to a string.
    /// </summary>
    /// <param name="c">The character to parse.</param>
    /// <returns>A parser that parses the given character and returns it as a string.</returns>
    public static IParser<string> OneCharText(char c) => OneChar(c).AsString();

    /// <summary>
    /// Returns a parser that matches a character satisfying the specified condition.
    /// </summary>
    /// <param name="condition">The condition that the character must satisfy.</param>
    /// <return>A parser that matches a character if it satisfies the specified condition.</return>
    public static IParser<string> Satisfy(Func<char, bool> condition) => new SatisfyParser(condition).AsString();

    /// <summary>
    /// Matches any character.
    /// </summary>
    /// <return>An instance of <see cref="IParser{T}"/> that matches any single character.</return>
    public static IParser<string> Any() => new SatisfyParser(_ => true).AsString();

    /// <summary>
    /// Creates a parser that matches any of the specified characters.
    /// </summary>
    /// <param name="chars">An array of characters to match against.</param>
    /// <return>
    /// An IParser instance that matches one or more instances of any of the specified characters.
    /// </return>
    public static IParser<string> AnyOf(params char[] chars)
    {
        var charSet = chars.ToHashSet();
        
        return ManySatisfy(charSet.Contains);
    }

    /// <summary>
    /// Returns a parser that matches a character that does not satisfy the specified condition.
    /// </summary>
    /// <param name="condition">The condition that the character must not satisfy.</param>
    /// <return>A parser that matches a character if it does not satisfy the specified condition.</return>
    public static IParser<string> NotSatisfy(Func<char, bool> condition) => Satisfy(c => !condition(c));

    /// <summary>
    /// Creates a parser that matches many characters that do not satisfy the given condition.
    /// </summary>
    /// <param name="condition">The condition that a character should not satisfy to be matched by the parser.</param>
    /// <return>A parser that matches many characters that do not satisfy the condition.</return>
    public static IParser<string> ManyExcept(Func<char, bool> condition) => ManySatisfy(c => !condition(c));

    /// <summary>
    /// Creates a parser that matches a single letter or digit character.
    /// </summary>
    /// <return>An <see cref="IParser{T}"/> that matches any letter or digit character.</return>
    public static IParser<char> IsLetterOrDigit() => new SatisfyParser(char.IsLetterOrDigit);

    /// <summary>
    /// Parses a sequence of characters consisting of letters or digits.
    /// </summary>
    /// <return>An <see cref="IParser{T}"/> that parses a sequence of letters or digits and returns them as a string.</return>
    public static IParser<string> ManyLettersOrDigits() => new ManyParser<char>(IsLetterOrDigit()).AsString();

    /// <summary>
    /// Applies the provided parser in a repetitive manner and collects the results into an enumerable sequence.
    /// </summary>
    /// <param name="parser">The parser to be applied repetitively.</param>
    /// <return>An IParser that parses multiple instances using the specified parser and returns an enumerable of the collected results.</return>
    public static IParser<IEnumerable<T>> Many<T>(IParser<T> parser) => new ManyParser<T>(parser);

    /// <summary>
    /// Creates a parser that matches zero or more characters satisfying the given condition.
    /// </summary>
    /// <param name="condition">The condition that each character must satisfy.</param>
    /// <return>A parser that yields a string of characters meeting the specified condition.</return>
    public static IParser<string> ManySatisfy(Func<char, bool> condition) => new ManyParser<char>(new SatisfyParser(condition)).AsString();

    /// <summary>
    /// Parses a sequence of items separated by a specified separator parser.
    /// </summary>
    /// <param name="itemParser">The parser for individual items.</param>
    /// <param name="separatorParser">The parser for separators between items.</param>
    /// <param name="matchTailingSeparator">If set to true, matches a trailing separator after the last item.</param>
    /// <returns>An IParser that parses a sequence of items separated by the specified separator.</returns>
    public static IParser<T[]> SeparatedBy<T>(this IParser<T> itemParser, IParser<string> separatorParser, bool matchTailingSeparator = false) 
        => new SeparatedByParser<T, string>(itemParser, separatorParser, matchTailingSeparator: matchTailingSeparator);

    /// <summary>
    /// Consumes input characters while the specified condition is met.
    /// </summary>
    /// <param name="condition">A function that determines if the consumption should continue based on the current input.</param>
    /// <return>An IParser that produces a string of consumed characters.</return>
    public static IParser<string> ConsumeWhile(Func<ReadOnlyMemory<char>, bool> condition) => new WhileTextParser(condition);

    /// <summary>
    /// Parses input between a specified left parser, middle parser, and right parser.
    /// </summary>
    /// <param name="left">The parser that matches the left boundary.</param>
    /// <param name="items">The parser that matches the content in between the left and right boundaries.</param>
    /// <param name="right">The parser that matches the right boundary.</param>
    /// <returns>A parser that matches content between the specified left and right parsers.</returns>
    public static BetweenParser<TLeft, T3, TRight> Between<TLeft, T3, TRight>(IParser<TLeft> left, IParser<T3> items, IParser<TRight> right) => new(left, items, right);

    /// <summary>
    /// Creates a parser that matches a string enclosed within the specified quote character.
    /// </summary>
    /// <param name="quoteChar">The character used to quote the string, typically a single or double quote.</param>
    /// <return>A parser that can match and parse a quoted string.</return>
    public static IParser<string> CreateStringParser(char quoteChar)
    {
        var contentParser = ConsumeWhile
        (
            match => match.Span.EndsWith($"\\{quoteChar}") || !match.Span.EndsWith(quoteChar.ToString())
        );
        var quote = OneCharText(quoteChar);

        var parser = from start in quote 
            from str in contentParser >> quote 
            select str;
        
        return parser.Select(s => s.Replace($"\\{quoteChar}", $"{quoteChar}"));
    }

    /// <summary>
    /// A parser that matches quoted text within either single or double quotes.
    /// </summary>
    /// <remarks>
    /// This parser is used to identify and parse text that is enclosed in
    /// single (`'`) or double (`"`) quotes. It is particularly useful in
    /// scenarios where quoted strings need to be extracted from larger
    /// text constructs, such as JSON or Markdown.
    /// </remarks>
    public static readonly IParser<string> QuotedTextParser = CreateStringParser('\'') | CreateStringParser('"');

    /// <summary>
    /// Creates a lazy parser that defers the creation of the actual parser until parsing begins.
    /// This is useful for recursive parsers where a parser needs to reference itself.
    /// </summary>
    /// <typeparam name="T">The type of value the parser produces.</typeparam>
    /// <param name="parserFactory">A factory function that creates the parser when needed.</param>
    /// <returns>A lazy parser that will invoke the factory function when parsing.</returns>
    public static IParser<T> Lazy<T>(Func<IParser<T>> parserFactory) => new LazyParser<T>(parserFactory);
}