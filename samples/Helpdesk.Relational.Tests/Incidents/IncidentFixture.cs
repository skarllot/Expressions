using System.Globalization;
using Helpdesk.Relational.Incidents;
using Helpdesk.Relational.Incidents.AssignAgent;
using Helpdesk.Relational.Incidents.LogNew;
using Helpdesk.Relational.Incidents.RecordAgentResponse;
using Helpdesk.Relational.Incidents.RecordCustomerResponse;
using Helpdesk.Relational.Incidents.Resolve;

namespace Helpdesk.Relational.Tests.Incidents;

public static class IncidentFixture
{
    private static readonly Guid s_incidentId = new("33bf175c-fdbf-448c-b449-f41afdeb54e3");
    private static readonly Guid s_customerId = new("fa01de1e-63cf-4aa6-b306-0cfbc158ae04");
    private static readonly Guid s_agentId = new("8bf24056-5157-4d3b-bac8-1efeac0c71f7");

    public static LogIncident LogNew()
    {
        return new LogIncident(
            s_incidentId,
            s_customerId,
            new Contact(ContactChannel.Phone, FirstName: "John"),
            "My laptop is not turning on anymore",
            s_agentId);
    }

    public static Incident Logged()
    {
        return Incident.Create(LogNewScenario.Execute(LogNew()));
    }

    public static Incident AgentAssigned()
    {
        Incident incident = Logged();

        incident.Apply(AssignAgentScenario.Execute(incident, new AssignAgentToIncident(s_incidentId, s_agentId)));

        return incident;
    }

    public static Incident Resolved()
    {
        Incident incident = AgentAssigned();

        incident.Apply(
            RecordAgentResponseScenario.Execute(
                incident,
                new RecordAgentResponseToIncident(
                    s_incidentId,
                    new IncidentResponse.FromAgent(
                        s_agentId,
                        "Sent a new laptop",
                        true,
                        DateTimeOffset.Parse("2023-09-07T16:20:02Z", CultureInfo.InvariantCulture)))));

        incident.Apply(
            RecordCustomerResponseScenario.Execute(
                incident,
                new RecordCustomerResponseToIncident(
                    s_incidentId,
                    new IncidentResponse.FromCustomer(
                        s_customerId,
                        "Laptop received",
                        DateTimeOffset.Parse("2023-09-08T13:00:15Z", CultureInfo.InvariantCulture)))));

        incident.Apply(
            RecordAgentResponseScenario.Execute(
                incident,
                new RecordAgentResponseToIncident(
                    s_incidentId,
                    new IncidentResponse.FromAgent(
                        s_agentId,
                        "Ok, closing",
                        true,
                        DateTimeOffset.Parse("2023-09-08T14:01:02Z", CultureInfo.InvariantCulture)))));

        incident.Apply(
            ResolveScenario.Execute(
                incident,
                new ResolveIncident(s_incidentId, ResolutionType.Permanent, s_customerId)));

        return incident;
    }
}
