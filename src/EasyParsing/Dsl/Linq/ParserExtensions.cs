using System.Collections.Immutable;
using EasyParsing.Parsers;

namespace EasyParsing.Dsl.Linq;

/// <summary>
/// Provides extension methods for IParser{T}.
/// </summary>
public static class ParserExtensions
{
    /// <summary>
    /// Projects each element of a parser sequence to another parser and
    /// flattens the resulting sequences into one sequence, then invokes a
    /// result selector function on each pair of correlated elements.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements in the first parser.</typeparam>
    /// <typeparam name="TSecond">The type of the elements in the second parser.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
    /// <param name="first">The first parser containing the source elements.</param>
    /// <param name="secondSelector">
    /// A function to apply to each element in the first parser that returns a second parser.
    /// </param>
    /// <param name="resultSelector">
    /// A function to apply to each pair of elements from the first and second parsers.
    /// </param>
    /// <returns>
    /// A parser containing a sequence of elements obtained by invoking the result selector function
    /// on each pair of correlated elements from the first and second parsers.
    /// </returns>
    public static IParser<TResult> SelectMany<TFirst, TSecond, TResult>(
        this IParser<TFirst> first,
        Func<TFirst, IParser<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        return new SelectManyParser<TFirst, TSecond, TResult>(first, secondSelector, resultSelector);
    }

    /// <summary>
    /// Projects each element of a parser sequence into a new form by incorporating the specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source parser.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
    /// <param name="source">The source parser containing the elements to project.</param>
    /// <param name="selector">A transform function to apply to each element in the source parser.</param>
    /// <returns>A parser containing the projected elements.</returns>
    public static IParser<TResult> Select<TSource, TResult>(
        this IParser<TSource> source,
        Func<TSource, TResult> selector)
    {
        return new SelectParser<TSource, TResult>(source, selector);
    }

    /// <summary>
    /// Filters the elements of a parser sequence based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source parser.</typeparam>
    /// <param name="source">The source parser to apply the predicate to.</param>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    /// <returns>
    /// A parser containing elements from the source parser that satisfy the condition specified by the predicate.
    /// </returns>
    public static IParser<T> Where<T>(this IParser<T> source, Func<T, bool> predicate)
    {
        return new WhereParser<T>(source, predicate);
    }

    /// <summary>
    /// Converts a parser that produces a sequence of characters into a parser that produces a string.
    /// </summary>
    /// <param name="source">The parser that produces a sequence of characters.</param>
    /// <returns>An <see cref="IParser{T}"/> that produces a string.</returns>
    public static IParser<string> AsString(this IParser<IEnumerable<char>> source)
    {
        return source.Select(chars => new string(chars.ToArray()));
    }

    /// <summary>
    /// Converts a parser of one char into a parser of string.
    /// </summary>
    /// <param name="source">The parser that produces a sequence of characters.</param>
    /// <returns>A parser that produces the concatenated string of the characters parsed by the source parser.</returns>
    public static IParser<string> AsString(this IParser<char> source)
    {
        return source.Select(c => c.ToString());
    }

    /// <summary>
    /// Applies an accumulator function over a sequence of elements parsed from the source parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source parser.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">The source parser that provides the sequence of elements.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">An accumulator function to be invoked on each element of the sequence.</param>
    /// <returns>A parser that contains the final accumulator value after processing the entire sequence.</returns>
    public static IParser<TAccumulate> Aggregate<TSource, TAccumulate>(
        this IParser<IEnumerable<TSource>> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        return source.Select(items =>
        {
            TAccumulate accumulate = seed;
            foreach (var item in items)
            {
                accumulate = func(accumulate, item);
            }
            return accumulate;
        });
    }
    
}