using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using System.IO;
using System.Text.Json;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class SpellFSDataManager : FSDataManager<SpellModel>
    {
        private record StatsDto(int Strength, int Intelligence, int Dexterity, int Defense, int Vitality);
        private record SpellDto(
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

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.SPELL_FILE_EXT);
        }

        public override SpellModel Load(IDataManagerArguments arguments)
        {
            var spellArguments = (arguments as ContentFileDataLoaderArguments);
            string json = File.ReadAllText(this.RootPath + spellArguments.FileName + EngineConstants.SPELL_FILE_EXT);
            var dto = JsonSerializer.Deserialize<SpellDto>(json, JsonOptions);

            var model = new SpellModel();
            model.Name = dto.Name;
            model.DisplaySprite = new SpriteInfo(dto.DisplaySpriteName);
            model.CastTime = dto.CastTime;
            model.ActiveTime = dto.ActiveTime;
            model.CooldownTime = dto.CooldownTime;
            model.HealthCost = dto.HealthCost;
            model.ManaCost = dto.ManaCost;
            model.CasterAnimationPath = dto.CasterAnimationPath;
            model.TargetAnimationPath = dto.TargetAnimationPath;
            model.StatModifiers = new Stats()
            {
                Strength = dto.StatModifiers.Strength,
                Intelligence = dto.StatModifiers.Intelligence,
                Dexterity = dto.StatModifiers.Dexterity,
                Defense = dto.StatModifiers.Defense,
                Vitality = dto.StatModifiers.Vitality,
            };
            model.StatRequirements = new Stats()
            {
                Strength = dto.StatRequirements.Strength,
                Intelligence = dto.StatRequirements.Intelligence,
                Dexterity = dto.StatRequirements.Dexterity,
                Defense = dto.StatRequirements.Defense,
                Vitality = dto.StatRequirements.Vitality,
            };
            model.BehaviorKey = dto.BehaviorKey;

            return model;
        }

        public override void Save(IContentModel contentModel, IDataManagerArguments arguments)
        {
            var model = (SpellModel)contentModel;
            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.SPELL_FILE_EXT;

            var dto = new SpellDto(
                model.Name,
                model.DisplaySprite?.TextureName ?? "",
                model.CastTime,
                model.ActiveTime,
                model.CooldownTime,
                model.HealthCost,
                model.ManaCost,
                model.CasterAnimationPath,
                model.TargetAnimationPath,
                new StatsDto(model.StatModifiers.Strength, model.StatModifiers.Intelligence, model.StatModifiers.Dexterity, model.StatModifiers.Defense, model.StatModifiers.Vitality),
                new StatsDto(model.StatRequirements.Strength, model.StatRequirements.Intelligence, model.StatRequirements.Dexterity, model.StatRequirements.Defense, model.StatRequirements.Vitality),
                model.BehaviorKey ?? ""
            );

            File.WriteAllText(filePath, JsonSerializer.Serialize(dto, JsonOptions));
        }
    }
}
