using AwesomeAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QueryStrategyTest
{
    [Fact]
    public void CreateShouldAlwaysReturnAll()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        var queryStrategy = QueryStrategy.AllOfEntity<string>();

        string?[] result1 = source
            .Apply(queryStrategy)
            .ToArray();
        string?[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .ToArray();

        result1.Should().Equal("john", "jane", "", "hugo", "jack");
        result2.Should().Equal("john", "jane", "", "hugo", "jack");
    }

    [Fact]
    public void CreateShouldEvaluateSpecificationCorrectly()
    {
        string[] source = { "john", "jane", "hugo", "jack" };
        var queryStrategy = QueryStrategy.CreateForEntity(new StringBeginsWithJohnSpecification());

        string[] result1 = source
            .Apply(queryStrategy)
            .ToArray();
        string[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .ToArray();

        result1.Should().Equal("john");
        result2.Should().Equal("john");
    }

    [Fact]
    public void CreateNestedShouldReturnAll()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        char[] expected = { 'j', 'o', 'h', 'n', 'j', 'a', 'n', 'e', 'h', 'u', 'g', 'o', 'j', 'a', 'c', 'k' };
        var queryStrategy = QueryStrategy.CreateNested((string s) => s.Select(l => Tuple.Create(l)));

        char[] result1 = source
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();
        char[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();

        result1.Should().Equal(expected);
        result2.Should().Equal(expected);
    }

    [Fact]
    public void CreateNestedWithSpecificationShouldReturnExpected()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        char[] expected = { 'j', 'o', 'n', 'j', 'n', 'u', 'o', 'j', 'k' };
        var queryStrategy = QueryStrategy.CreateNested(
            (string s) => s.Select(l => Tuple.Create(l)),
            Specification.Create<Tuple<char>>(t => t.Item1 > 'h'));

        char[] result1 = source
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();
        char[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();

        result1.Should().Equal(expected);
        result2.Should().Equal(expected);
    }

    [Fact]
    public void CreateNestedWithEntityQueryStrategyShouldReturnExpected()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        char[] expected = { 'j', 'o', 'n', 'j', 'n', 'u', 'o', 'j', 'k' };
        var queryStrategy = QueryStrategy.CreateNested(
            (string s) => s.Select(l => Tuple.Create(l)),
            QueryStrategy.CreateForEntity((IQueryable<Tuple<char>> s) => s.Where(t => t.Item1 > 'h')));

        char[] result1 = source
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();
        char[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .Select(t => t.Item1)
            .ToArray();

        result1.Should().Equal(expected);
        result2.Should().Equal(expected);
    }
}
