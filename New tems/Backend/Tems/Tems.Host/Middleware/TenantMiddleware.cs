using Tems.Common.Tenant;

namespace Tems.Host.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private const string TenantHeaderName = "X-Tenant-Id";

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // Extract tenant ID from header
        if (context.Request.Headers.TryGetValue(TenantHeaderName, out var tenantIdValues))
        {
            var tenantId = tenantIdValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                ((TenantContext)tenantContext).TenantId = tenantId;
            }
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
