namespace Raiqub.Expressions.Common;

internal static class EnumerableExtensions
{
    public static TSource AggregateOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, TSource, TSource> func,
        TSource defaultValue)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();
        if (!e.MoveNext())
        {
            return defaultValue;
        }

        TSource result = e.Current;
        while (e.MoveNext())
        {
            result = func(result, e.Current);
        }

        return result;
    }

    public static TSource AggregateOrDefault<TSource>(
        this ReadOnlySpan<TSource> source,
        Func<TSource, TSource, TSource> func,
        TSource defaultValue)
    {
        var e = source.GetEnumerator();
        if (!e.MoveNext())
        {
            return defaultValue;
        }

        TSource result = e.Current;
        while (e.MoveNext())
        {
            result = func(result, e.Current);
        }

        return result;
    }
}
