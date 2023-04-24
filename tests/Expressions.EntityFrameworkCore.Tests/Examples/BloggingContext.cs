using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DefineBlog(modelBuilder.Entity<Blog>());
        DefinePost(modelBuilder.Entity<Post>());

        modelBuilder.AddSqliteDateTimeOffsetConverter(Database);
    }

    private static void DefineBlog(EntityTypeBuilder<Blog> builder)
    {
        builder.Property(b => b.Id);
        builder.Property(b => b.Name);

        builder.HasKey(b => b.Id);
        builder.HasMany(b => b.Posts).WithOne().HasForeignKey("BlogId");

#if NET5_0_OR_GREATER
        builder.Navigation(b => b.Posts).AutoInclude();
#endif
    }

    private static void DefinePost(EntityTypeBuilder<Post> builder)
    {
        builder.Property<Guid>("Id");
        builder.Property<Guid>("BlogId");
        builder.Property(p => p.Title);
        builder.Property(p => p.Content);

        builder.HasKey("Id");

        builder.HasDiscriminator<string>("PostType")
            .HasValue<Post>("Post")
            .HasValue<VideoPost>("VideoPost");
    }
}
