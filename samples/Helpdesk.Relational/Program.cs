using System.Text.Json.Serialization;
using Helpdesk.Relational;
using Helpdesk.Relational.Incidents.Acknowledge.v1;
using Helpdesk.Relational.Incidents.AssignAgent.v1;
using Helpdesk.Relational.Incidents.Categorise.v1;
using Helpdesk.Relational.Incidents.Close.v1;
using Helpdesk.Relational.Incidents.GetCustomerSummary.v1;
using Helpdesk.Relational.Incidents.GetDetails.v1;
using Helpdesk.Relational.Incidents.GetHistory.v1;
using Helpdesk.Relational.Incidents.GetShortInfo.v1;
using Helpdesk.Relational.Incidents.LogNew.v1;
using Helpdesk.Relational.Incidents.Prioritise.v1;
using Helpdesk.Relational.Incidents.RecordAgentResponse.v1;
using Helpdesk.Relational.Incidents.RecordCustomerResponse.v1;
using Helpdesk.Relational.Incidents.Resolve.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.EntityFrameworkCore;
using RT.Comb;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton(Provider.PostgreSql)
    .Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(
        options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Entity Framework
builder.Services
    .AddDbContextFactory<HelpdeskDbContext>(ob => ob.UseNpgsql(builder.Configuration.GetConnectionString("Incidents")))
    .AddEntityFrameworkExpressions()
    .AddSingleContext<HelpdeskDbContext>();

// Handlers
builder.Services
    .AddScoped<LogIncidentUseCase>()
    .AddScoped<CategoriseIncidentUseCase>()
    .AddScoped<PrioritiseIncidentUseCase>()
    .AddScoped<AssignAgentToIncidentUseCase>()
    .AddScoped<RecordCustomerResponseToIncidentUseCase>()
    .AddScoped<RecordAgentResponseToIncidentUseCase>()
    .AddScoped<ResolveIncidentUseCase>()
    .AddScoped<AcknowledgeIncidentResolutionUseCase>()
    .AddScoped<CloseIncidentUseCase>()
    .AddScoped<GetIncidentShortInfoUseCase>()
    .AddScoped<GetIncidentDetailsUseCase>()
    .AddScoped<GetIncidentHistoryUseCase>()
    .AddScoped<GetCustomerIncidentsSummaryUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI();
}

app.MapPost(
        "api/customers/{customerId:guid}/incidents/",
        async (LogIncidentUseCase useCase, Guid customerId, LogIncidentRequest request, CancellationToken ct) =>
        {
            var incident = await useCase.Execute(customerId, request, ct);
            return Results.Created($"api/incidents/{incident.Id}", incident.Id);
        })
    .WithTags("Customer");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/category",
        (
                CategoriseIncidentUseCase useCase,
                Guid incidentId,
                Guid agentId,
                CategoriseIncidentRequest request,
                CancellationToken ct) =>
            useCase.Execute(incidentId, agentId, request, ct))
    .WithTags("Agent");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/priority",
        (
            PrioritiseIncidentUseCase useCase,
            Guid incidentId,
            Guid agentId,
            PrioritiseIncidentRequest request,
            CancellationToken ct) => useCase.Execute(incidentId, agentId, request, ct))
    .WithTags("Agent");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/assign",
        (
            AssignAgentToIncidentUseCase useCase,
            Guid incidentId,
            Guid agentId,
            CancellationToken ct) => useCase.Execute(incidentId, agentId, ct))
    .WithTags("Agent");

app.MapPost(
        "api/customers/{customerId:guid}/incidents/{incidentId:guid}/responses/",
        (
            RecordCustomerResponseToIncidentUseCase useCase,
            Guid incidentId,
            Guid customerId,
            RecordCustomerResponseToIncidentRequest request,
            CancellationToken ct) => useCase.Execute(incidentId, customerId, request, ct))
    .WithTags("Customer");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/responses/",
        (
            RecordAgentResponseToIncidentUseCase useCase,
            Guid incidentId,
            Guid agentId,
            RecordAgentResponseToIncidentRequest request,
            CancellationToken ct) => useCase.Execute(incidentId, agentId, request, ct))
    .WithTags("Agent");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/resolve",
        (
            ResolveIncidentUseCase useCase,
            Guid incidentId,
            Guid agentId,
            ResolveIncidentRequest request,
            CancellationToken ct) => useCase.Execute(incidentId, agentId, request, ct))
    .WithTags("Agent");

app.MapPost(
        "api/customers/{customerId:guid}/incidents/{incidentId:guid}/acknowledge",
        (
            AcknowledgeIncidentResolutionUseCase useCase,
            Guid incidentId,
            Guid customerId,
            CancellationToken ct) => useCase.Execute(incidentId, customerId, ct))
    .WithTags("Customer");

app.MapPost(
        "api/agents/{agentId:guid}/incidents/{incidentId:guid}/close",
        async (
            CloseIncidentUseCase useCase,
            Guid incidentId,
            Guid agentId,
            CancellationToken ct) =>
        {
            await useCase.Execute(incidentId, agentId, ct);
            return Results.Ok();
        })
    .WithTags("Agent");

app.MapGet(
        "api/customers/{customerId:guid}/incidents/",
        (
                GetIncidentShortInfoUseCase useCase,
                Guid customerId,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize,
                CancellationToken ct) =>
            useCase.Execute(customerId, pageNumber, pageSize, ct))
    .WithTags("Customer");

app.MapGet(
        "api/incidents/{incidentId:guid}",
        (GetIncidentDetailsUseCase useCase, Guid incidentId, CancellationToken ct) =>
            useCase.Execute(new GetIncidentDetailsRequest(incidentId), ct))
    .WithTags("Incident");

app.MapGet(
        "api/incidents/{incidentId:guid}/history",
        (GetIncidentHistoryUseCase useCase, Guid incidentId, CancellationToken ct) => useCase.Execute(incidentId, ct))
    .WithTags("Incident");

app.MapGet(
        "api/customers/{customerId:guid}/incidents-summary",
        (GetCustomerIncidentsSummaryUseCase useCase, Guid customerId, CancellationToken ct) =>
            useCase.Execute(customerId, ct))
    .WithTags("Customer");

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<HelpdeskDbContext>().Database.EnsureCreated();
}

app.Run();
