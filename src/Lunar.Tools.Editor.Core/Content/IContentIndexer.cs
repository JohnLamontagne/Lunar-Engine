using Lunar.Tools.Editor.Contracts.Content;

namespace Lunar.Tools.Editor.Core.Content;

public interface IContentIndexer
{
    /// <summary>
    /// Builds the project content tree from disk.
    /// Throws InvalidOperationException when no project is open.
    /// </summary>
    ContentTreeNodeDto BuildTree();
}
