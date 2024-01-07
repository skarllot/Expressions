using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class JohnQueryStrategy : EntityQueryStrategy<string>
{
    public JohnQueryStrategy(params Specification<string>[] restrictions)
        : base(restrictions)
    {
    }

    public JohnQueryStrategy(IEnumerable<Specification<string>> restrictions)
        : base(restrictions)
    {
    }

    protected override IQueryable<string> ExecuteCore(IQueryable<string> source)
    {
        return source.Where(x => x.StartsWith("john", StringComparison.InvariantCultureIgnoreCase));
    }
}
