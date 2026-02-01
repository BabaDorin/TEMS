using AssetManagement.API;
using LocationManagement.Api;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tems.Common.Tenant;
using Tems.Host.Middleware;
using Tems.Host.Seeding;
using TicketManagement.API;
using UserManagement.API;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Ensure appsettings.Development.json is loaded for non-production environments
if (!builder.Environment.IsProduction())
{
    builder.Environment.EnvironmentName = "Development";
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}

// Register Tenant Context as scoped
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Add modules first
// builder.Services.AddExampleServices(builder.Configuration);
builder.Services.AddAssetManagementServices(builder.Configuration);
builder.Services.AddLocationManagementModule();
builder.Services.AddTicketManagementServices(builder.Configuration);
builder.Services.AddUserManagementServices(builder.Configuration);

// Add JWT Bearer Authentication - Validate tokens from Keycloak
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keycloak issues tokens, not IdentityServer directly
        options.Authority = builder.Configuration["Keycloak:Authority"] 
            ?? "http://localhost:8080/realms/tems";
        options.Audience = "account"; // Keycloak default audience
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = new[] { "account", "tems-api" },
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", // Microsoft claim type
            NameClaimType = "preferred_username"
        };
        options.RequireHttpsMetadata = false; // Only for dev
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>();
                logger.LogInformation("Token validated. Claims: {Claims}", string.Join(", ", claims));
                
                var roles = context.Principal?.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    .Select(c => c.Value) ?? Array.Empty<string>();
                logger.LogInformation("User roles: {Roles}", string.Join(", ", roles));
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization Policies - Using roles from Keycloak
builder.Services.AddAuthorization(options =>
{
    // Asset Management
    options.AddPolicy("CanManageAssets", policy =>
        policy.RequireRole("can_manage_assets"));
    
    // Ticket Management
    options.AddPolicy("CanManageTickets", policy =>
        policy.RequireRole("can_manage_tickets"));
        
    options.AddPolicy("CanOpenTickets", policy =>
        policy.RequireRole("can_open_tickets"));
    
    // User Management
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireRole("can_manage_users"));
});

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration["Cors:AllowedOrigins"]?.Split(';') 
                ?? new[] { "http://localhost:4200" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add FastEndpoints - scan all assemblies including module assemblies
builder.Services.AddFastEndpoints(options =>
{
    options.Assemblies = new[]
    {
        typeof(Program).Assembly,
        typeof(AssetManagementServiceRegistration).Assembly,
        typeof(TicketManagementServiceRegistration).Assembly,
        typeof(UserManagementServiceRegistration).Assembly
    };
});

// Add Swagger
builder.Services.SwaggerDocument();

// Add Database Seeder Service
builder.Services.AddHostedService<DatabaseSeederService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseTenantMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();