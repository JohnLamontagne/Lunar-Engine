namespace Lunar.Tools.Editor.Contracts.Documents;

/// <summary>
/// Editor-safe authoring model for a spell (.spell JSON file).
/// </summary>
public record SpellEditorDocument(
    string FilePath,
    string Name,
    string DisplaySpriteName,
    int CastTime,
    int ActiveTime,
    int CooldownTime,
    int HealthCost,
    int ManaCost,
    string CasterAnimationPath,
    string TargetAnimationPath,
    StatsDto StatModifiers,
    StatsDto StatRequirements,
    string BehaviorKey
);
