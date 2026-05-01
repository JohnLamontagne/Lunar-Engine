using Lunar.Tools.Editor.Contracts.Projects;
using Lunar.Tools.Editor.Core.Projects;

namespace Lunar.Tools.Editor.Api.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/project");

        group.MapGet("/", (IProjectRepository repo) =>
            repo.CurrentProject is { } p ? Results.Ok(p) : Results.NoContent());

        group.MapPost("/open", (OpenProjectRequest req, IProjectRepository repo) =>
        {
            try { return Results.Ok(repo.Open(req)); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/create", (CreateProjectRequest req, IProjectRepository repo) =>
        {
            try { return Results.Ok(repo.Create(req)); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        return app;
    }
}
