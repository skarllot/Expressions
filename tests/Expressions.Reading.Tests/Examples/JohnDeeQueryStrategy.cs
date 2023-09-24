using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class JohnDeeQueryStrategy : EntityQueryStrategy<string, (string Value, int Length)>
{
    public JohnDeeQueryStrategy()
    {
    }

    public JohnDeeQueryStrategy(params Specification<string>[] restrictions)
        : base(restrictions)
    {
    }

    protected override IEnumerable<Specification<string>> GetPreconditions()
    {
        yield return new StringBeginsWithJohnSpecification();
        yield return new StringEndsWithDeeSpecification();
    }

    protected override IQueryable<(string Value, int Length)> ExecuteCore(IQueryable<string> source)
    {
        return source
            .Select(it => new ValueTuple<string, int>(it, it.Length));
    }
}
