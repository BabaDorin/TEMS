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