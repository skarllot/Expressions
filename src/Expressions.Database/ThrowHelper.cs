using System.Diagnostics.CodeAnalysis;

namespace Raiqub.Expressions;

internal static class ThrowHelper
{
#if !NETSTANDARD2_0
    [DoesNotReturn]
#endif
    internal static void ThrowMoreThanOneElementException() =>
        throw new InvalidOperationException("Sequence contains more than one element");
}
