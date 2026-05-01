using Lunar.Tools.Editor.Contracts.Documents;
using Lunar.Tools.Editor.Core.Spells;

namespace Lunar.Tools.Editor.Api.Endpoints;

public static class SpellEndpoints
{
    public static IEndpointRouteBuilder MapSpellEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/spells");

        group.MapGet("/load", (string path, ISpellRepository repo) =>
        {
            try { return Results.Ok(repo.Load(path)); }
            catch (FileNotFoundException) { return Results.NotFound(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/save", (SpellEditorDocument doc, ISpellRepository repo) =>
        {
            try { repo.Save(doc); return Results.Ok(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/create", (CreateSpellRequest req, ISpellRepository repo) =>
        {
            try { return Results.Ok(repo.Create(req)); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapDelete("/", (string path, ISpellRepository repo) =>
        {
            try { repo.Delete(path); return Results.Ok(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        return app;
    }
}
