# Split Queries

When working with Entity Framework, you may encounter situations where loading related entities in a single query can lead to a "cartesian explosion". This happens when multiple related entities are included in the query, and the resulting dataset becomes excessively large due to cartesian products, impacting performance.

To mitigate this issue, Entity Framework Core allows you to specify the use of split queries for loading related entities. Split queries separate the loading of each related entity into its own query, preventing the cartesian explosion problem.

You can enable split queries for a specific entity by configuring the query options in your application's service configuration. Here's how you can do it:

```csharp
services.AddEntityFrameworkExpressions()
    .Configure<Blog>(options => options.UseSplitQuery = true)
    .AddSingleContext<YourDbContext>();
```

In this example:
- `services.AddEntityFrameworkExpressions()` registers the necessary services for Entity Framework Expressions.
- `.Configure<Blog>(options => options.UseSplitQuery = true)` configures the split query option for the Blog entity. Replace _Blog_ with the entity type for which you want to enable split queries.
- `.AddSingleContext<YourDbContext>()` adds the Entity Framework context to your application.

## When to Use Split Queries
Split queries are particularly useful when you have large datasets or complex relationships between entities, and you want to optimize performance by avoiding the cartesian explosion issue. However, enabling split queries should be considered on a case-by-case basis, as it may introduce additional database queries.

Keep in mind that using split queries is just one of many performance optimization techniques available in Entity Framework. Careful consideration of your data model, query patterns, and database design is essential for achieving optimal performance.

By configuring split queries when appropriate, you can strike a balance between efficient data retrieval and preventing performance bottlenecks caused by cartesian explosions.

For more information refer to [Single vs. Split Queries](https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries) article.