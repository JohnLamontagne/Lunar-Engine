using Microsoft.Extensions.DependencyInjection;
using Lunar.Tools.Editor.Core.Content;
using Lunar.Tools.Editor.Core.Items;
using Lunar.Tools.Editor.Core.Projects;
using Lunar.Tools.Editor.Core.Scripts;
using Lunar.Tools.Editor.Core.Spells;

namespace Lunar.Tools.Editor.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEditorCore(this IServiceCollection services)
    {
        services.AddSingleton<IProjectRepository, ProjectRepository>();
        services.AddSingleton<IContentIndexer, ContentIndexer>();
        services.AddSingleton<IScriptRepository, ScriptRepository>();
        services.AddSingleton<ScriptCompileService>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<ISpellRepository, SpellRepository>();
        return services;
    }
}
