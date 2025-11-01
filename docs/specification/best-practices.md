# Best Practices

## Error Handling and Edge Cases

When working with specifications, consider these common scenarios:

### Null Safety

```csharp
public static class ProductSpecification
{
    // Handle null strings safely
    public static Specification<Product> HasCategory(string categoryName) =>
        Specification.Create<Product>(p =>
            p.Category != null && p.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

    // Handle null navigation properties
    public static Specification<Product> HasSupplier =>
        Specification.Create<Product>(p => p.Supplier != null);

    // Combine for safe navigation
    public static Specification<Product> FromSupplierInCountry(string country) =>
        HasSupplier.And(Specification.Create<Product>(
            p => p.Supplier!.Country.Equals(country, StringComparison.OrdinalIgnoreCase)));
}
```

### Database Translation Limitations

Some C# expressions cannot be translated to SQL. Keep specifications simple and database-friendly:

::: code-group

```csharp [❌ Bad - Won't Translate]
// Using complex string methods that may not translate to SQL
public static Specification<Product> HasComplexNamePattern { get; } =
    Specification.Create<Product>(p =>
        p.Name.Split(',').Any(part => part.Trim().StartsWith("Pro")));
```

```csharp [✅ Good - Translates Well]
// Using simple, translatable operations
public static Specification<Product> NameStartsWithPro { get; } =
    Specification.Create<Product>(p => p.Name.StartsWith("Pro"));

public static Specification<Product> NameContainsPro { get; } =
    Specification.Create<Product>(p => p.Name.Contains("Pro"));
```

:::
