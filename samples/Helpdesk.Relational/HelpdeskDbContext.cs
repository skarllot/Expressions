using Helpdesk.Relational.Incidents;
using Helpdesk.Relational.Incidents.Categorise;
using Helpdesk.Relational.Incidents.LogNew;
using Helpdesk.Relational.Incidents.Prioritise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Helpdesk.Relational;

public class HelpdeskDbContext : DbContext
{
    public HelpdeskDbContext(DbContextOptions<HelpdeskDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var incidentBuilder = modelBuilder.Entity<Incident>();

        incidentBuilder.Property(i => i.Id);
        incidentBuilder.Property(i => i.CustomerId);
        incidentBuilder.Property(i => i.Description);
        incidentBuilder.Property(i => i.LoggedBy);
        incidentBuilder.Property(i => i.LoggedAt);
        incidentBuilder.Property(i => i.Status).HasConversion<EnumToStringConverter<IncidentStatus>>();
        incidentBuilder.Property(i => i.Category).HasConversion<EnumToStringConverter<IncidentCategory>>();
        incidentBuilder.Property(i => i.Priority).HasConversion<EnumToStringConverter<IncidentPriority>>();
        incidentBuilder.HasKey(i => i.Id);

        incidentBuilder.OwnsOne(i => i.Contact)
            .Property(c => c.ContactChannel).HasConversion<EnumToStringConverter<ContactChannel>>();
        incidentBuilder.HasMany(i => i.Responses).WithOne();

        incidentBuilder.Navigation(i => i.Responses).AutoInclude();

        var responseBuilder = modelBuilder.Entity<IncidentResponse>();
        responseBuilder.Property<int>("Id");
        responseBuilder.HasKey("Id");

        responseBuilder.HasDiscriminator<string>("type")
            .HasValue<IncidentResponse.FromAgent>("agent")
            .HasValue<IncidentResponse.FromCustomer>("customer");
    }
}
