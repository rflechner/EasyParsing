namespace EasyParsing;

/// <summary>
/// Represents an abstract base class for options that represents an optional value.
/// </summary>
/// <typeparam name="T">The type of the value that might be contained in the option.</typeparam>
public abstract record Option<T>;

/// <summary>
/// Represents a class that wraps a value of type <typeparamref name="T"/> as an instance of <see cref="Option{T}"/>.
/// Used to indicate that a valid value is present.
/// </summary>
/// <typeparam name="T">The type of the value being wrapped.</typeparam>
public record Some<T>(T Value) : Option<T>;

/// <summary>
/// Represents the absence of a value in an optional context.
/// Used to indicate that no valid value is present.
/// </summary>
/// <typeparam name="T">The type of the value that would be absent.</typeparam>
public record None<T> : Option<T>;

/// <summary>
/// Provides extension methods for working with <see cref="Option{T}"/> instances.
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Retrieves the value of the specified <see cref="Option{T}"/> if it is a <see cref="Some{T}"/>; otherwise, returns the specified default value.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the option.</typeparam>
    /// <param name="option">The option from which to get the value or default.</param>
    /// <param name="defaultValue">The value to return if the option is a <see cref="None{T}"/>.</param>
    /// <returns>The value contained in the option if it is a <see cref="Some{T}"/>; otherwise, the specified default value.</returns>
    public static T? GetValueOrDefault<T>(this Option<T> option, T? defaultValue = default)
    {
        switch (option)
        {
            case None<T>:
                return defaultValue;
            case Some<T> some:
                return some.Value;
            default:
                throw new ArgumentOutOfRangeException(nameof(option));
        }
    }
}
