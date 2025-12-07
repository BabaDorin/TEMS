namespace Tems.Common.Tenant;

public class TenantContext : ITenantContext
{
    public string TenantId { get; set; } = "default";
}
