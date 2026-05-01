using Lunar.Tools.Editor.Contracts.Documents;
using Lunar.Tools.Editor.Core.Items;

namespace Lunar.Tools.Editor.Api.Endpoints;

public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items");

        group.MapGet("/load", (string path, IItemRepository repo) =>
        {
            try { return Results.Ok(repo.Load(path)); }
            catch (FileNotFoundException) { return Results.NotFound(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/save", (ItemEditorDocument doc, IItemRepository repo) =>
        {
            try { repo.Save(doc); return Results.Ok(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/create", (CreateItemRequest req, IItemRepository repo) =>
        {
            try { return Results.Ok(repo.Create(req)); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapDelete("/", (string path, IItemRepository repo) =>
        {
            try { repo.Delete(path); return Results.Ok(); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        return app;
    }
}
