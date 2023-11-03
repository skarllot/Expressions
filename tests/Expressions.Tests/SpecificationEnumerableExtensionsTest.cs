using FluentAssertions;
using Raiqub.Expressions.Tests.Common;

namespace Raiqub.Expressions.Tests;

public class SpecificationEnumerableExtensionsTest
{
    private static readonly Random s_random = new(1447398856);

    public static IEnumerable<string> RandomWords1 => new[]
    {
        "computer", "shower", "auditor", "beam", "joy", "similar", "inch", "access", "acute", "evoke", "pitch",
        "planet", "pudding", "directory", "rider"
    };

    public static IEnumerable<string> RandomWords2 => new[]
    {
        "sanctuary", "traction", "finance", "promotion", "peel", "illusion", "domination", "turkey", "relate",
        "change", "software", "economist", "attention", "ribbon", "pierce"
    };

    public static IEnumerable<string> RandomWords1Big => RandomWords1
        .Concat(RandomWords1.OrderBy(_ => s_random.Next()))
        .Concat(RandomWords1.OrderBy(_ => s_random.Next()))
        .Concat(RandomWords1.OrderBy(_ => s_random.Next()))
        .Concat(RandomWords1.OrderBy(_ => s_random.Next()))
        .OrderBy(_ => s_random.Next());

    public static TheoryData<string> WordsIn1 => TheoryBuilder.Build(RandomWords1.ToArray());
    public static TheoryData<string> WordsIn2 => TheoryBuilder.Build(RandomWords2.ToArray());

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void AnyShouldBeTrueWhenMatchFound(string word)
    {
        bool hasWord = RandomWords1.Any(Specification.Create<string>(s => s == word));

        hasWord.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void AnyShouldBeFalseWhenMatchNotFound(string word)
    {
        bool hasWord = RandomWords1.Any(Specification.Create<string>(s => s == word));

        hasWord.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void AnyShouldBeFalseWhenCollectionIsEmpty(string word)
    {
        bool hasWord = Enumerable.Empty<string>().Any(Specification.Create<string>(s => s == word));

        hasWord.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void CountShouldReturnExpectedWhenMatchFound(string word)
    {
        int count1 = RandomWords1.Count(Specification.Create<string>(s => s == word));
        int count2 = RandomWords1Big.Count(Specification.Create<string>(s => s == word));

        count1.Should().Be(1);
        count2.Should().Be(5);
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void CountShouldBeZeroWhenMatchNotFound(string word)
    {
        int count = RandomWords1Big.Count(Specification.Create<string>(s => s == word));

        count.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void CountShouldBeZeroWhenCollectionIsEmpty(string word)
    {
        int count = Enumerable.Empty<string>().Count(Specification.Create<string>(s => s == word));

        count.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void FirstShouldReturnExpectedWhenMatchFound(string word)
    {
        string found1 = RandomWords1.First(Specification.Create<string>(s => s == word));
        string found2 = RandomWords1Big.First(Specification.Create<string>(s => s == word));

        found1.Should().Be(word);
        found2.Should().Be(word);
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void FirstShouldThrowWhenMatchNotFound(string word)
    {
        Action first = () => RandomWords1Big.First(Specification.Create<string>(s => s == word));

        first.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void FirstShouldThrowWhenCollectionIsEmpty(string word)
    {
        Action first = () => Enumerable.Empty<string>().First(Specification.Create<string>(s => s == word));

        first.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void FirstOrDefaultShouldReturnExpectedWhenMatchFound(string word)
    {
        string? found1 = RandomWords1.FirstOrDefault(Specification.Create<string>(s => s == word));
        string? found2 = RandomWords1Big.FirstOrDefault(Specification.Create<string>(s => s == word));

        found1.Should().Be(word);
        found2.Should().Be(word);
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void FirstOrDefaultShouldThrowWhenMatchNotFound(string word)
    {
        string? found = RandomWords1Big.FirstOrDefault(Specification.Create<string>(s => s == word));

        found.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void FirstOrDefaultShouldThrowWhenCollectionIsEmpty(string word)
    {
        string? found = Enumerable.Empty<string>().FirstOrDefault(Specification.Create<string>(s => s == word));

        found.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleShouldReturnExpectedWhenSingleMatchIsFound(string word)
    {
        string found = RandomWords1.Single(Specification.Create<string>(s => s == word));

        found.Should().Be(word);
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void SingleShouldThrowWhenMatchNotFound(string word)
    {
        Action single = () => RandomWords1.Single(Specification.Create<string>(s => s == word));

        single.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleShouldThrowsWhenMultipleMatchesAreFound(string word)
    {
        Action single = () => RandomWords1Big.Single(Specification.Create<string>(s => s == word));

        single.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleShouldThrowsWhenCollectionIsEmpty(string word)
    {
        Action single = () => Enumerable.Empty<string>().Single(Specification.Create<string>(s => s == word));

        single.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleOrDefaultShouldReturnExpectedWhenSingleMatchIsFound(string word)
    {
        string? found = RandomWords1.SingleOrDefault(Specification.Create<string>(s => s == word));

        found.Should().Be(word);
    }

    [Theory]
    [MemberData(nameof(WordsIn2))]
    public void SingleOrDefaultShouldBeNullWhenMatchNotFound(string word)
    {
        string? found = RandomWords1.SingleOrDefault(Specification.Create<string>(s => s == word));

        found.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleOrDefaultShouldThrowsWhenMultipleMatchesAreFound(string word)
    {
        Action single = () => RandomWords1Big.SingleOrDefault(Specification.Create<string>(s => s == word));

        single.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(WordsIn1))]
    public void SingleOrDefaultShouldThrowsWhenCollectionIsEmpty(string word)
    {
        string? found = Enumerable.Empty<string>().SingleOrDefault(Specification.Create<string>(s => s == word));

        found.Should().BeNull();
    }
}
