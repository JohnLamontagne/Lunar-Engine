using Lunar.Tools.Editor.Contracts.Documents;

namespace Lunar.Tools.Editor.Core.Items;

public interface IItemRepository
{
    ItemEditorDocument Load(string absolutePath);
    ItemEditorDocument Create(CreateItemRequest request);
    void Save(ItemEditorDocument document);
    void Delete(string absolutePath);
}
