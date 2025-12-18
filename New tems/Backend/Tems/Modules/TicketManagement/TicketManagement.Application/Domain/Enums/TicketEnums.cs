namespace TicketManagement.Application.Domain.Enums;

public enum ItilCategory
{
    Incident,
    Problem,
    Change,
    Request,
    SecurityIncident,
    Alert
}

public enum StateType
{
    Open,
    Wip,
    Pending,
    Resolved,
    Closed
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public enum ChannelSource
{
    Teams,
    Slack,
    Web
}

public enum SenderType
{
    User,
    Agent,
    AiSystem
}

public enum AttributeDataType
{
    String,
    Number,
    Bool,
    Date,
    User,
    AssetRef
}
