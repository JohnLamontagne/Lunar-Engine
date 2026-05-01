using System.Text.Json;
using Lunar.Tools.Editor.Contracts.Documents;

namespace Lunar.Tools.Editor.Core.Spells;

public sealed class SpellRepository : ISpellRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private record StatFileDto(int Strength, int Intelligence, int Dexterity, int Defense, int Vitality);

    private record SpellFileDto(
        string Name,
        string DisplaySpriteName,
        int CastTime,
        int ActiveTime,
        int CooldownTime,
        int HealthCost,
        int ManaCost,
        string CasterAnimationPath,
        string TargetAnimationPath,
        StatFileDto StatModifiers,
        StatFileDto StatRequirements,
        string BehaviorKey
    );

    public SpellEditorDocument Load(string absolutePath)
    {
        string json = File.ReadAllText(absolutePath);
        var dto = JsonSerializer.Deserialize<SpellFileDto>(json, JsonOptions)
            ?? throw new InvalidDataException($"Failed to parse spell file: {absolutePath}");

        return new SpellEditorDocument(
            FilePath: absolutePath,
            Name: dto.Name,
            DisplaySpriteName: dto.DisplaySpriteName,
            CastTime: dto.CastTime,
            ActiveTime: dto.ActiveTime,
            CooldownTime: dto.CooldownTime,
            HealthCost: dto.HealthCost,
            ManaCost: dto.ManaCost,
            CasterAnimationPath: dto.CasterAnimationPath,
            TargetAnimationPath: dto.TargetAnimationPath,
            StatModifiers: ToStatsDto(dto.StatModifiers),
            StatRequirements: ToStatsDto(dto.StatRequirements),
            BehaviorKey: dto.BehaviorKey
        );
    }

    public SpellEditorDocument Create(CreateSpellRequest request)
    {
        string filePath = Path.Combine(request.DirPath, request.Name + ".spell");
        var empty = new StatsDto(0, 0, 0, 0, 0);
        var doc = new SpellEditorDocument(
            FilePath: filePath,
            Name: request.Name,
            DisplaySpriteName: "",
            CastTime: 0, ActiveTime: 0, CooldownTime: 0,
            HealthCost: 0, ManaCost: 0,
            CasterAnimationPath: "", TargetAnimationPath: "",
            StatModifiers: empty, StatRequirements: empty,
            BehaviorKey: ""
        );
        Save(doc);
        return doc;
    }

    public void Save(SpellEditorDocument document)
    {
        var dto = new SpellFileDto(
            document.Name, document.DisplaySpriteName,
            document.CastTime, document.ActiveTime, document.CooldownTime,
            document.HealthCost, document.ManaCost,
            document.CasterAnimationPath, document.TargetAnimationPath,
            ToFileDto(document.StatModifiers),
            ToFileDto(document.StatRequirements),
            document.BehaviorKey
        );
        File.WriteAllText(document.FilePath, JsonSerializer.Serialize(dto, JsonOptions));
    }

    public void Delete(string absolutePath) => File.Delete(absolutePath);

    private static StatsDto ToStatsDto(StatFileDto d) =>
        new(d.Strength, d.Intelligence, d.Dexterity, d.Defense, d.Vitality);

    private static StatFileDto ToFileDto(StatsDto d) =>
        new(d.Strength, d.Intelligence, d.Dexterity, d.Defense, d.Vitality);
}
