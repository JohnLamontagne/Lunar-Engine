using Lunar.Tools.Editor.Core.Content;

namespace Lunar.Tools.Editor.Api.Endpoints;

public static class ContentEndpoints
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/content");

        group.MapGet("/tree", (IContentIndexer indexer) =>
        {
            try { return Results.Ok(indexer.BuildTree()); }
            catch (InvalidOperationException ex) { return Results.Problem(ex.Message, statusCode: 400); }
        });

        return app;
    }
}
