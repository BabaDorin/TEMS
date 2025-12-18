using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

public class WorkflowConfig
{
    [BsonElement("states")]
    public List<WorkflowState> States { get; set; } = new();

    [BsonElement("initial_state_id")]
    public string InitialStateId { get; set; } = string.Empty;
}
