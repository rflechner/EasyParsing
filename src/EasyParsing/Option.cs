namespace EasyParsing;

public abstract record Option<T>;

public record Some<T>(T Value) : Option<T>;

public record None<T> : Option<T>;

public static class OptionExtensions
{
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
