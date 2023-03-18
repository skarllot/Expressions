namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public static class BlogSpecification
{
    public static Specification<Blog> OfName(string name) => Specification.Create<Blog>(b => b.Name == name);
}
