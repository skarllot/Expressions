using AwesomeAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QueryStrategyExtensionsTest
{
    private class TestQuerySource : IQuerySource
    {
        private readonly Dictionary<Type, object> _sets = new();

        public void AddSet<TEntity>(IEnumerable<TEntity> data)
            where TEntity : class
        {
            _sets[typeof(TEntity)] = data.AsQueryable();
        }

        public IQueryable<TEntity> GetSet<TEntity>()
            where TEntity : class
        {
            if (_sets.TryGetValue(typeof(TEntity), out var set))
            {
                return (IQueryable<TEntity>)set;
            }
            return Enumerable.Empty<TEntity>().AsQueryable();
        }
    }

    [Fact]
    public void ThenShouldChainTwoStrategiesCorrectly()
    {
        // Arrange
        var posts = new[]
        {
            new BlogPost(1, "First", "The first post", "general"),
            new BlogPost(2, "Second", "The second post", "speech"),
            new BlogPost(3, "Third", "Another post", "general"),
        };

        // Strategy 1: Filter by post type
        var firstStrategy = QueryStrategy.CreateForEntity<BlogPost, BlogPost>(s =>
            s.Where(p => p.PostType == "general")
        );

        // Strategy 2: Project to titles
        var secondStrategy = QueryStrategy.CreateForEntity<BlogPost, string>(s => s.Select(p => p.Title));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert
        result1.Should().Equal("First", "Third");
        result2.Should().Equal("First", "Third");
    }

    [Fact]
    public void ThenShouldChainMultipleTransformations()
    {
        // Arrange
        var posts = new[]
        {
            new BlogPost(1, "First", "The first post", "general"),
            new BlogPost(2, "Second", "The second post", "speech"),
            new BlogPost(3, "Third", "Another post", "general"),
        };

        // Strategy 1: Filter by post type
        var filterStrategy = QueryStrategy.CreateForEntity<BlogPost, BlogPost>(s =>
            s.Where(p => p.PostType == "general")
        );

        // Strategy 2: Project to uppercase titles
        var projectStrategy = QueryStrategy.CreateForEntity<BlogPost, string>(s => s.Select(p => p.Title.ToUpper()));

        // Act
        var combinedStrategy = filterStrategy.Then(projectStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert
        result1.Should().Equal("FIRST", "THIRD");
        result2.Should().Equal("FIRST", "THIRD");
    }

    [Fact]
    public void ThenShouldWorkWithSpecificationBasedStrategies()
    {
        // Arrange
        var posts = new[]
        {
            new BlogPost(1, "First", "The first post", "general"),
            new BlogPost(2, "Second", "The second post content", "speech"),
            new BlogPost(3, "Third", "Another second item", "general"),
        };

        var firstStrategy = QueryStrategy.CreateForEntity(new BlogPostContentSearchSpecification("second"));
        var secondStrategy = QueryStrategy.CreateForEntity<BlogPost, int>(s => s.Select(p => p.Id));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert
        result1.Should().Equal(2, 3);
        result2.Should().Equal(2, 3);
    }

    [Fact]
    public void ThenShouldHandleEmptyResults()
    {
        // Arrange
        var posts = new[]
        {
            new BlogPost(1, "First", "The first post", "general"),
            new BlogPost(2, "Second", "The second post", "speech"),
        };

        var firstStrategy = QueryStrategy.CreateForEntity<BlogPost, BlogPost>(s => s.Where(p => p.PostType == "video"));

        var secondStrategy = QueryStrategy.CreateForEntity<BlogPost, string>(s => s.Select(p => p.Title));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert
        result1.Should().BeEmpty();
        result2.Should().BeEmpty();
    }

    [Fact]
    public void ThenShouldPreserveIntermediateTypeCorrectly()
    {
        // Arrange
        var posts = new[]
        {
            new NewsPost(1, "First", "Content", "news", "John"),
            new NewsPost(2, "Second", "More content", "news", "Jane"),
            new NewsPost(3, "Third", "Other content", "editorial", "Bob"),
        };

        // Strategy 1: NewsPost -> NewsPost (filter by type)
        var firstStrategy = QueryStrategy.CreateForEntity<NewsPost, NewsPost>(s => s.Where(p => p.PostType == "news"));

        // Strategy 2: NewsPost -> string (get author)
        var secondStrategy = QueryStrategy.CreateForEntity<NewsPost, string>(s => s.Select(p => p.Author));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert
        result1.Should().Equal("John", "Jane");
        result2.Should().Equal("John", "Jane");
    }

    [Fact]
    public void ThenShouldWorkWithThreeChainedStrategies()
    {
        // Arrange
        var posts = new[]
        {
            new NewsPost(1, "First", "The first post", "news", "John"),
            new NewsPost(2, "Second", "The second post", "news", "Jane"),
            new NewsPost(3, "Third", "Another post", "editorial", "Bob"),
        };

        // Strategy 1: Filter by post type
        var filterStrategy = QueryStrategy.CreateForEntity<NewsPost, NewsPost>(s => s.Where(p => p.PostType == "news"));

        // Strategy 2: Project to author
        var authorStrategy = QueryStrategy.CreateForEntity<NewsPost, string>(s => s.Select(p => p.Author));

        // Strategy 3: Count characters
        var lengthStrategy = QueryStrategy.CreateForEntity<string, int>(s => s.Select(name => name.Length));

        // Act
        var combinedStrategy = filterStrategy.Then(authorStrategy).Then(lengthStrategy);

        var result1 = posts.AsQueryable().Apply(combinedStrategy).ToArray();

        var result2 = posts.Apply(combinedStrategy).ToArray();

        // Assert - "John" = 4, "Jane" = 4
        result1.Should().Equal(4, 4);
        result2.Should().Equal(4, 4);
    }

    [Fact]
    public void ThenShouldChainQueryStrategyWithEntityQueryStrategy()
    {
        // Arrange
        var querySource = new TestQuerySource();
        querySource.AddSet(
            new[]
            {
                new BlogPost(1, "First", "The first post", "general"),
                new BlogPost(2, "Second", "The second post", "speech"),
                new BlogPost(3, "Third", "Another post", "general"),
            }
        );

        // Strategy 1: IQueryStrategy that gets BlogPost entities and filters them
        var firstStrategy = QueryStrategy.Create<BlogPost>(source =>
            source.GetSet<BlogPost>().Where(p => p.PostType == "general")
        );

        // Strategy 2: IEntityQueryStrategy that projects to titles
        var secondStrategy = QueryStrategy.CreateForEntity<BlogPost, string>(s => s.Select(p => p.Title));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);
        var result = combinedStrategy.Execute(querySource).ToArray();

        // Assert
        result.Should().Equal("First", "Third");
    }

    [Fact]
    public void ThenShouldChainQueryStrategyWithTransformation()
    {
        // Arrange
        var querySource = new TestQuerySource();
        querySource.AddSet(
            new[]
            {
                new NewsPost(1, "First", "Content", "news", "John"),
                new NewsPost(2, "Second", "More content", "news", "Jane"),
                new NewsPost(3, "Third", "Other content", "editorial", "Bob"),
            }
        );

        // Strategy 1: IQueryStrategy that gets filtered NewsPost entities
        var firstStrategy = QueryStrategy.Create<NewsPost>(source =>
            source.GetSet<NewsPost>().Where(p => p.PostType == "news")
        );

        // Strategy 2: IEntityQueryStrategy that projects to author and counts length
        var secondStrategy = QueryStrategy.CreateForEntity<NewsPost, int>(s => s.Select(p => p.Author.Length));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);
        var result = combinedStrategy.Execute(querySource).ToArray();

        // Assert - "John" = 4, "Jane" = 4
        result.Should().Equal(4, 4);
    }

    [Fact]
    public void ThenShouldHandleEmptyResultsForQueryStrategy()
    {
        // Arrange
        var querySource = new TestQuerySource();
        querySource.AddSet(
            new[]
            {
                new BlogPost(1, "First", "The first post", "general"),
                new BlogPost(2, "Second", "The second post", "speech"),
            }
        );

        // Strategy 1: IQueryStrategy that filters to empty result
        var firstStrategy = QueryStrategy.Create<BlogPost>(source =>
            source.GetSet<BlogPost>().Where(p => p.PostType == "video")
        );

        // Strategy 2: IEntityQueryStrategy that projects to titles
        var secondStrategy = QueryStrategy.CreateForEntity<BlogPost, string>(s => s.Select(p => p.Title));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);
        var result = combinedStrategy.Execute(querySource).ToArray();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ThenShouldWorkWithMultipleEntityTypesInQuerySource()
    {
        // Arrange
        var querySource = new TestQuerySource();
        querySource.AddSet(
            new[]
            {
                new BlogPost(1, "First", "The first post", "general"),
                new BlogPost(2, "Second", "The second post", "speech"),
            }
        );
        querySource.AddSet(
            new[]
            {
                new NewsPost(10, "News1", "News content", "news", "Alice"),
                new NewsPost(20, "News2", "More news", "news", "Bob"),
            }
        );

        // Strategy 1: IQueryStrategy that gets NewsPost entities
        var firstStrategy = QueryStrategy.Create<NewsPost>(source => source.GetSet<NewsPost>());

        // Strategy 2: IEntityQueryStrategy that projects to author
        var secondStrategy = QueryStrategy.CreateForEntity<NewsPost, string>(s => s.Select(p => p.Author));

        // Act
        var combinedStrategy = firstStrategy.Then(secondStrategy);
        var result = combinedStrategy.Execute(querySource).ToArray();

        // Assert
        result.Should().Equal("Alice", "Bob");
    }
}
