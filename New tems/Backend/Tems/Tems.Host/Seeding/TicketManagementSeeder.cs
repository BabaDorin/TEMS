using MongoDB.Driver;
using TicketManagement.Infrastructure.Entities;

namespace Tems.Host.Seeding;

public class TicketManagementSeeder(IMongoDatabase database, ILogger<TicketManagementSeeder> logger)
{
    private readonly IMongoCollection<TicketType> _ticketTypes = database.GetCollection<TicketType>("ticket_types");
    private readonly IMongoCollection<Ticket> _tickets = database.GetCollection<Ticket>("tickets");

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding Ticket Management data...");

        await SeedTicketTypesAsync();
        await SeedTicketsAsync();

        logger.LogInformation("Ticket Management seeding completed.");
    }

    private async Task SeedTicketTypesAsync()
    {
        var count = await _ticketTypes.CountDocumentsAsync(FilterDefinition<TicketType>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Ticket types already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding ticket types...");

        var ticketTypes = new List<TicketType>
        {
            new()
            {
                TicketTypeId = "ticket_type_hardware_issue",
                Name = "Hardware Issue",
                Description = "Hardware malfunction or failure",
                ItilCategory = "incident",
                Version = 1,
                WorkflowConfig = new WorkflowConfig
                {
                    InitialStateId = "state_new",
                    States =
                    [
                        new WorkflowState
                        {
                            Id = "state_new",
                            Label = "New",
                            Type = "initial",
                            AllowedTransitions = ["state_assigned", "state_closed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_assigned",
                            Label = "Assigned",
                            Type = "intermediate",
                            AllowedTransitions = ["state_in_progress", "state_new", "state_closed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_in_progress",
                            Label = "In Progress",
                            Type = "intermediate",
                            AllowedTransitions = ["state_waiting", "state_resolved"]
                        },
                        new WorkflowState
                        {
                            Id = "state_waiting",
                            Label = "Waiting for Parts",
                            Type = "intermediate",
                            AllowedTransitions = ["state_in_progress", "state_closed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_resolved",
                            Label = "Resolved",
                            Type = "intermediate",
                            AllowedTransitions = ["state_closed", "state_in_progress"]
                        },
                        new WorkflowState
                        {
                            Id = "state_closed",
                            Label = "Closed",
                            Type = "final",
                            AllowedTransitions = []
                        }
                    ]
                },
                AttributeDefinitions =
                [
                    new AttributeDefinition
                    {
                        Key = "affected_equipment",
                        Label = "Affected Equipment",
                        DataType = "string",
                        IsRequired = true,
                        IsPredefined = true,
                        AiExtractionHint = "The equipment or device experiencing the issue"
                    },
                    new AttributeDefinition
                    {
                        Key = "issue_type",
                        Label = "Issue Type",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["Won't Power On", "Performance Issue", "Physical Damage", "Connectivity Problem", "Other"],
                        AiExtractionHint = "Category of hardware problem"
                    },
                    new AttributeDefinition
                    {
                        Key = "location",
                        Label = "Equipment Location",
                        DataType = "string",
                        IsRequired = false,
                        IsPredefined = true,
                        AiExtractionHint = "Physical location of the equipment"
                    }
                ]
            },
            new()
            {
                TicketTypeId = "ticket_type_software_request",
                Name = "Software Request",
                Description = "Software installation or license request",
                ItilCategory = "service_request",
                Version = 1,
                WorkflowConfig = new WorkflowConfig
                {
                    InitialStateId = "state_new",
                    States =
                    [
                        new WorkflowState
                        {
                            Id = "state_new",
                            Label = "New",
                            Type = "initial",
                            AllowedTransitions = ["state_approval_pending", "state_rejected"]
                        },
                        new WorkflowState
                        {
                            Id = "state_approval_pending",
                            Label = "Pending Approval",
                            Type = "intermediate",
                            AllowedTransitions = ["state_approved", "state_rejected"]
                        },
                        new WorkflowState
                        {
                            Id = "state_approved",
                            Label = "Approved",
                            Type = "intermediate",
                            AllowedTransitions = ["state_in_progress"]
                        },
                        new WorkflowState
                        {
                            Id = "state_in_progress",
                            Label = "In Progress",
                            Type = "intermediate",
                            AllowedTransitions = ["state_completed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_completed",
                            Label = "Completed",
                            Type = "final",
                            AllowedTransitions = []
                        },
                        new WorkflowState
                        {
                            Id = "state_rejected",
                            Label = "Rejected",
                            Type = "final",
                            AllowedTransitions = []
                        }
                    ]
                },
                AttributeDefinitions =
                [
                    new AttributeDefinition
                    {
                        Key = "software_name",
                        Label = "Software Name",
                        DataType = "string",
                        IsRequired = true,
                        IsPredefined = true,
                        AiExtractionHint = "Name of the software being requested"
                    },
                    new AttributeDefinition
                    {
                        Key = "business_justification",
                        Label = "Business Justification",
                        DataType = "text",
                        IsRequired = true,
                        IsPredefined = true,
                        AiExtractionHint = "Reason why this software is needed"
                    },
                    new AttributeDefinition
                    {
                        Key = "license_type",
                        Label = "License Type",
                        DataType = "enum",
                        IsRequired = false,
                        IsPredefined = true,
                        Options = ["Single User", "Multi-User", "Site License", "Subscription"],
                        AiExtractionHint = "Type of software license needed"
                    }
                ]
            },
            new()
            {
                TicketTypeId = "ticket_type_access_request",
                Name = "Access Request",
                Description = "Request for system or resource access",
                ItilCategory = "service_request",
                Version = 1,
                WorkflowConfig = new WorkflowConfig
                {
                    InitialStateId = "state_new",
                    States =
                    [
                        new WorkflowState
                        {
                            Id = "state_new",
                            Label = "New",
                            Type = "initial",
                            AllowedTransitions = ["state_approval_pending", "state_denied"]
                        },
                        new WorkflowState
                        {
                            Id = "state_approval_pending",
                            Label = "Pending Manager Approval",
                            Type = "intermediate",
                            AllowedTransitions = ["state_approved", "state_denied"]
                        },
                        new WorkflowState
                        {
                            Id = "state_approved",
                            Label = "Approved",
                            Type = "intermediate",
                            AllowedTransitions = ["state_provisioning"]
                        },
                        new WorkflowState
                        {
                            Id = "state_provisioning",
                            Label = "Provisioning Access",
                            Type = "intermediate",
                            AllowedTransitions = ["state_granted"]
                        },
                        new WorkflowState
                        {
                            Id = "state_granted",
                            Label = "Access Granted",
                            Type = "final",
                            AllowedTransitions = []
                        },
                        new WorkflowState
                        {
                            Id = "state_denied",
                            Label = "Denied",
                            Type = "final",
                            AllowedTransitions = []
                        }
                    ]
                },
                AttributeDefinitions =
                [
                    new AttributeDefinition
                    {
                        Key = "system_name",
                        Label = "System/Resource",
                        DataType = "string",
                        IsRequired = true,
                        IsPredefined = true,
                        AiExtractionHint = "The system or resource access is needed for"
                    },
                    new AttributeDefinition
                    {
                        Key = "access_level",
                        Label = "Access Level",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["Read Only", "Read/Write", "Administrator", "Custom"],
                        AiExtractionHint = "Level of access required"
                    },
                    new AttributeDefinition
                    {
                        Key = "duration",
                        Label = "Access Duration",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["Temporary (30 days)", "Temporary (90 days)", "Permanent"],
                        AiExtractionHint = "How long access is needed"
                    }
                ]
            },
            new()
            {
                TicketTypeId = "ticket_type_network_issue",
                Name = "Network Issue",
                Description = "Network connectivity or performance problems",
                ItilCategory = "incident",
                Version = 1,
                WorkflowConfig = new WorkflowConfig
                {
                    InitialStateId = "state_new",
                    States =
                    [
                        new WorkflowState
                        {
                            Id = "state_new",
                            Label = "New",
                            Type = "initial",
                            AllowedTransitions = ["state_investigating", "state_closed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_investigating",
                            Label = "Investigating",
                            Type = "intermediate",
                            AllowedTransitions = ["state_identified", "state_escalated"]
                        },
                        new WorkflowState
                        {
                            Id = "state_identified",
                            Label = "Issue Identified",
                            Type = "intermediate",
                            AllowedTransitions = ["state_fixing"]
                        },
                        new WorkflowState
                        {
                            Id = "state_fixing",
                            Label = "Fixing",
                            Type = "intermediate",
                            AllowedTransitions = ["state_resolved", "state_escalated"]
                        },
                        new WorkflowState
                        {
                            Id = "state_escalated",
                            Label = "Escalated",
                            Type = "intermediate",
                            AllowedTransitions = ["state_fixing", "state_resolved"]
                        },
                        new WorkflowState
                        {
                            Id = "state_resolved",
                            Label = "Resolved",
                            Type = "intermediate",
                            AllowedTransitions = ["state_closed", "state_investigating"]
                        },
                        new WorkflowState
                        {
                            Id = "state_closed",
                            Label = "Closed",
                            Type = "final",
                            AllowedTransitions = []
                        }
                    ]
                },
                AttributeDefinitions =
                [
                    new AttributeDefinition
                    {
                        Key = "issue_type",
                        Label = "Issue Type",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["No Connectivity", "Slow Connection", "Intermittent Connection", "Wi-Fi Issue", "VPN Issue"],
                        AiExtractionHint = "Type of network problem"
                    },
                    new AttributeDefinition
                    {
                        Key = "affected_users",
                        Label = "Number of Affected Users",
                        DataType = "enum",
                        IsRequired = false,
                        IsPredefined = true,
                        Options = ["Single User", "Multiple Users", "Department", "Entire Office"],
                        AiExtractionHint = "Scope of the network issue"
                    }
                ]
            },
            new()
            {
                TicketTypeId = "ticket_type_password_reset",
                Name = "Password Reset",
                Description = "Password reset or account unlock request",
                ItilCategory = "service_request",
                Version = 1,
                WorkflowConfig = new WorkflowConfig
                {
                    InitialStateId = "state_new",
                    States =
                    [
                        new WorkflowState
                        {
                            Id = "state_new",
                            Label = "New",
                            Type = "initial",
                            AllowedTransitions = ["state_verifying"]
                        },
                        new WorkflowState
                        {
                            Id = "state_verifying",
                            Label = "Verifying Identity",
                            Type = "intermediate",
                            AllowedTransitions = ["state_processing", "state_rejected"]
                        },
                        new WorkflowState
                        {
                            Id = "state_processing",
                            Label = "Processing",
                            Type = "intermediate",
                            AllowedTransitions = ["state_completed"]
                        },
                        new WorkflowState
                        {
                            Id = "state_completed",
                            Label = "Completed",
                            Type = "final",
                            AllowedTransitions = []
                        },
                        new WorkflowState
                        {
                            Id = "state_rejected",
                            Label = "Rejected",
                            Type = "final",
                            AllowedTransitions = []
                        }
                    ]
                },
                AttributeDefinitions =
                [
                    new AttributeDefinition
                    {
                        Key = "request_type",
                        Label = "Request Type",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["Password Reset", "Account Unlock", "Both"],
                        AiExtractionHint = "Type of account request"
                    },
                    new AttributeDefinition
                    {
                        Key = "affected_system",
                        Label = "System",
                        DataType = "enum",
                        IsRequired = true,
                        IsPredefined = true,
                        Options = ["Windows Domain", "Email", "VPN", "Application", "Other"],
                        AiExtractionHint = "Which system the password/account is for"
                    }
                ]
            }
        };

        await _ticketTypes.InsertManyAsync(ticketTypes);
        logger.LogInformation("Seeded {Count} ticket types.", ticketTypes.Count);
    }

    private async Task SeedTicketsAsync()
    {
        var count = await _tickets.CountDocumentsAsync(FilterDefinition<Ticket>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Tickets already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding sample tickets...");

        var now = DateTime.UtcNow;
        var tickets = new List<Ticket>
        {
            new()
            {
                HumanReadableId = "TEMS-2024-001",
                TicketTypeId = "ticket_type_hardware_issue",
                Summary = "Laptop won't power on - LAP-001",
                Priority = "high",
                CurrentStateId = "state_in_progress",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                AssigneeId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                Attributes = new Dictionary<string, object>
                {
                    ["affected_equipment"] = "Laptop LAP-001",
                    ["issue_type"] = "Won't Power On",
                    ["location"] = "Main Office - Room 101"
                },
                CreatedAt = now.AddDays(-2)
            },
            new()
            {
                HumanReadableId = "TEMS-2024-002",
                TicketTypeId = "ticket_type_software_request",
                Summary = "Request for Adobe Creative Cloud license",
                Priority = "medium",
                CurrentStateId = "state_approval_pending",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                Attributes = new Dictionary<string, object>
                {
                    ["software_name"] = "Adobe Creative Cloud",
                    ["business_justification"] = "Creating marketing materials for Q2 product launch",
                    ["license_type"] = "Subscription"
                },
                CreatedAt = now.AddDays(-1)
            },
            new()
            {
                HumanReadableId = "TEMS-2024-003",
                TicketTypeId = "ticket_type_network_issue",
                Summary = "Cannot connect to Wi-Fi network",
                Priority = "high",
                CurrentStateId = "state_investigating",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                AssigneeId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                Attributes = new Dictionary<string, object>
                {
                    ["issue_type"] = "Wi-Fi Issue",
                    ["affected_users"] = "Single User"
                },
                CreatedAt = now.AddHours(-5)
            },
            new()
            {
                HumanReadableId = "TEMS-2024-004",
                TicketTypeId = "ticket_type_password_reset",
                Summary = "Password reset for email account",
                Priority = "high",
                CurrentStateId = "state_completed",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                AssigneeId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                Attributes = new Dictionary<string, object>
                {
                    ["request_type"] = "Password Reset",
                    ["affected_system"] = "Email"
                },
                CreatedAt = now.AddDays(-3),
                ResolvedAt = now.AddDays(-3).AddHours(2)
            },
            new()
            {
                HumanReadableId = "TEMS-2024-005",
                TicketTypeId = "ticket_type_access_request",
                Summary = "Request access to Project Management system",
                Priority = "medium",
                CurrentStateId = "state_approval_pending",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                Attributes = new Dictionary<string, object>
                {
                    ["system_name"] = "Project Management System",
                    ["access_level"] = "Read/Write",
                    ["duration"] = "Permanent"
                },
                CreatedAt = now.AddHours(-12)
            },
            new()
            {
                HumanReadableId = "TEMS-2024-006",
                TicketTypeId = "ticket_type_hardware_issue",
                Summary = "Monitor flickering intermittently - MON-002",
                Priority = "low",
                CurrentStateId = "state_new",
                Reporter = new Reporter { UserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625", ChannelSource = "web" },
                Attributes = new Dictionary<string, object>
                {
                    ["affected_equipment"] = "Monitor MON-002",
                    ["issue_type"] = "Performance Issue",
                    ["location"] = "Main Office - Room 101"
                },
                CreatedAt = now.AddHours(-3)
            }
        };

        await _tickets.InsertManyAsync(tickets);
        logger.LogInformation("Seeded {Count} sample tickets.", tickets.Count);
    }
}
