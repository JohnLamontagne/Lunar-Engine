using Lunar.Tools.Editor.Contracts.Scripts;

namespace Lunar.Tools.Editor.Core.Scripts;

public interface IScriptRepository
{
    ScriptDocumentDto Load(string absolutePath);
    void Save(string absolutePath, string content);
}
