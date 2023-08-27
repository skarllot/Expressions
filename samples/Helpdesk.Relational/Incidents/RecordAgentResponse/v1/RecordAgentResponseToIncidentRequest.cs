namespace Helpdesk.Relational.Incidents.RecordAgentResponse.v1;

public record RecordAgentResponseToIncidentRequest(
    string Content,
    bool VisibleToCustomer);
