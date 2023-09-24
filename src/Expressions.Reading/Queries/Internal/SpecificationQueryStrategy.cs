namespace Raiqub.Expressions.Queries.Internal;

internal sealed class SpecificationQueryStrategy<T> : IEntityQueryStrategy<T>
    where T : class
{
    private readonly Specification<T> _specification;

    public SpecificationQueryStrategy(Specification<T> specification) => _specification = specification;

    public IQueryable<T> Execute(IQueryable<T> source) => source.Where(_specification);

    public IEnumerable<T> Execute(IEnumerable<T> source) => source.Where(_specification);

    public IQueryable<T> Execute(IQuerySource source) => source.GetSet<T>().Where(_specification);
}
