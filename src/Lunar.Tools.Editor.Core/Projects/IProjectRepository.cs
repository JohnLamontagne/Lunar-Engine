using Lunar.Tools.Editor.Contracts.Projects;

namespace Lunar.Tools.Editor.Core.Projects;

public interface IProjectRepository
{
    ProjectManifest? CurrentProject { get; }
    ProjectManifest Open(OpenProjectRequest request);
    ProjectManifest Create(CreateProjectRequest request);
}
