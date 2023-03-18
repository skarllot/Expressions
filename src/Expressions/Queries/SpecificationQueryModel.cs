namespace Raiqub.Expressions.Queries;

internal sealed class SpecificationQueryModel<T> : IQueryModel<T>
{
    private readonly Specification<T> _specification;

    public SpecificationQueryModel(Specification<T> specification) => _specification = specification;

    public IQueryable<T> Execute(IQueryable<T> source) => source.Where(_specification);

    public IEnumerable<T> Execute(IEnumerable<T> source) => source.Where(_specification);
}
