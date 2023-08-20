namespace Raiqub.Expressions.Queries.Internal;

internal sealed class SpecificationQueryModel<T> : IEntityQueryModel<T>, IQueryModel<T>
    where T : class
{
    private readonly Specification<T> _specification;

    public SpecificationQueryModel(Specification<T> specification) => _specification = specification;

    public IQueryable<T> Execute(IQueryable<T> source) => source.Where(_specification);

    public IEnumerable<T> Execute(IEnumerable<T> source) => source.Where(_specification);

    public IQueryable<T> Execute(IQuerySource source) => source.GetSet<T>().Where(_specification);
}
