using Lunar.Tools.Editor.Contracts.Scripts;
using Lunar.Tools.Editor.Core.Scripts;

namespace Lunar.Tools.Editor.Api.Endpoints;

public static class ScriptEndpoints
{
    public static IEndpointRouteBuilder MapScriptEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/scripts");

        // GET /api/scripts/load?path=<absolute-path>
        group.MapGet("/load", (string path, IScriptRepository repo) =>
        {
            try { return Results.Ok(repo.Load(path)); }
            catch (FileNotFoundException) { return Results.NotFound(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        // POST /api/scripts/save
        group.MapPost("/save", (SaveScriptRequest req, IScriptRepository repo) =>
        {
            try
            {
                repo.Save(req.FilePath, req.Content);
                return Results.Ok();
            }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        // POST /api/scripts/compile
        group.MapPost("/compile", (ScriptCompileService svc) =>
        {
            try { return Results.Ok(svc.Compile()); }
            catch (InvalidOperationException ex) { return Results.Problem(ex.Message, statusCode: 400); }
        });

        return app;
    }
}
