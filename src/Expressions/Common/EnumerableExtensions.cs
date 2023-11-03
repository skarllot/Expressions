namespace Raiqub.Expressions.Common;

internal static class EnumerableExtensions
{
    public static TSource? AggregateOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, TSource, TSource> func)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();
        if (!e.MoveNext())
        {
            return default;
        }

        TSource result = e.Current;
        while (e.MoveNext())
        {
            result = func(result, e.Current);
        }

        return result;
    }
}
