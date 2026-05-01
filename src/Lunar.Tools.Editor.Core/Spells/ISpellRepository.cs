using Lunar.Tools.Editor.Contracts.Documents;

namespace Lunar.Tools.Editor.Core.Spells;

public interface ISpellRepository
{
    SpellEditorDocument Load(string absolutePath);
    SpellEditorDocument Create(CreateSpellRequest request);
    void Save(SpellEditorDocument document);
    void Delete(string absolutePath);
}
