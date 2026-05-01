using Lunar.Tools.Editor.Api.Endpoints;
using Lunar.Tools.Editor.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEditorCore();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapProjectEndpoints();
app.MapContentEndpoints();
app.MapScriptEndpoints();
app.MapItemEndpoints();
app.MapSpellEndpoints();

app.Run();
