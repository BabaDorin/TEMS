using EquipmentManagement.API;
using FastEndpoints;
using FastEndpoints.Swagger;
using Tems.Example.API;

var builder = WebApplication.CreateBuilder(args);

// Add modules first
// builder.Services.AddExampleServices(builder.Configuration);
builder.Services.AddEquipmentManagementServices(builder.Configuration);

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Add Swagger
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseFastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();