using EquipmentManagement.API;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tems.Example.API;

var builder = WebApplication.CreateBuilder(args);

// Ensure appsettings.Development.json is loaded for non-production environments
if (!builder.Environment.IsProduction())
{
    builder.Environment.EnvironmentName = "Development";
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}

// Add modules first
// builder.Services.AddExampleServices(builder.Configuration);
builder.Services.AddEquipmentManagementServices(builder.Configuration);

// Add JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.Audience = "tems-api";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
        options.RequireHttpsMetadata = false; // Only for dev
    });

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewEntities", policy =>
        policy.RequireClaim("can_view_entities", "true"));
    
    options.AddPolicy("CanManageEntities", policy =>
        policy.RequireClaim("can_manage_entities", "true"));
        
    options.AddPolicy("CanAllocateKeys", policy =>
        policy.RequireClaim("can_allocate_keys", "true"));
        
    options.AddPolicy("CanSendEmails", policy =>
        policy.RequireClaim("can_send_emails", "true"));
        
    options.AddPolicy("CanManageAnnouncements", policy =>
        policy.RequireClaim("can_manage_announcements", "true"));
        
    options.AddPolicy("CanManageSystemConfiguration", policy =>
        policy.RequireClaim("can_manage_system_configuration", "true"));
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

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Add Swagger
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();