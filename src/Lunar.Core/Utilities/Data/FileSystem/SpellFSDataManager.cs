using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World;
using System.IO;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class SpellFSDataManager : FSDataManager<SpellModel>
    {
        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.SPELL_FILE_EXT);
        }

        public override SpellModel Load(IDataManagerArguments arguments)
        {
            var spellArguments = (arguments as ContentFileDataLoaderArguments);

            var model = new SpellModel();

            using (var fileStream = new FileStream(this.RootPath + spellArguments.FileName + EngineConstants.SPELL_FILE_EXT, FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    model.Name = binaryReader.ReadString();
                    model.DisplaySprite = new SpriteInfo(binaryReader.ReadString());
                    model.CastTime = binaryReader.ReadInt32();
                    model.ActiveTime = binaryReader.ReadInt32();
                    model.CooldownTime = binaryReader.ReadInt32();

                    model.HealthCost = binaryReader.ReadInt32();
                    model.ManaCost = binaryReader.ReadInt32();

                    model.CasterAnimationPath = binaryReader.ReadString();
                    model.TargetAnimationPath = binaryReader.ReadString();

                    model.StatModifiers.Strength = binaryReader.ReadInt32();
                    model.StatModifiers.Intelligence = binaryReader.ReadInt32();
                    model.StatModifiers.Dexterity = binaryReader.ReadInt32();
                    model.StatModifiers.Defense = binaryReader.ReadInt32();
                    model.StatModifiers.Vitality = binaryReader.ReadInt32();

                    model.StatRequirements.Strength = binaryReader.ReadInt32();
                    model.StatRequirements.Intelligence = binaryReader.ReadInt32();
                    model.StatRequirements.Dexterity = binaryReader.ReadInt32();
                    model.StatRequirements.Defense = binaryReader.ReadInt32();
                    model.StatRequirements.Vitality = binaryReader.ReadInt32();

                    int scriptCount = binaryReader.ReadInt32();
                    for (int i = 0; i < scriptCount; i++)
                    {
                        model.Scripts.Add(binaryReader.ReadString(), binaryReader.ReadString());
                    }
                }
            }

            return model;
        }

        public override void Save(IContentModel contentModel, IDataManagerArguments arguments)
        {
            var model = (SpellModel)contentModel;

            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.SPELL_FILE_EXT;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(model.Name);

                    if (model.DisplaySprite != null)
                        binaryWriter.Write(model.DisplaySprite.TextureName);
                    else
                        binaryWriter.Write("");

                    binaryWriter.Write(model.CastTime);
                    binaryWriter.Write(model.ActiveTime);
                    binaryWriter.Write(model.CooldownTime);

                    binaryWriter.Write(model.HealthCost);
                    binaryWriter.Write(model.ManaCost);

                    binaryWriter.Write(model.CasterAnimationPath);
                    binaryWriter.Write(model.TargetAnimationPath);

                    binaryWriter.Write(model.StatModifiers.Strength);
                    binaryWriter.Write(model.StatModifiers.Intelligence);
                    binaryWriter.Write(model.StatModifiers.Dexterity);
                    binaryWriter.Write(model.StatModifiers.Defense);
                    binaryWriter.Write(model.StatModifiers.Vitality);

                    binaryWriter.Write(model.StatRequirements.Strength);
                    binaryWriter.Write(model.StatRequirements.Intelligence);
                    binaryWriter.Write(model.StatRequirements.Dexterity);
                    binaryWriter.Write(model.StatRequirements.Defense);
                    binaryWriter.Write(model.StatRequirements.Vitality);

                    binaryWriter.Write(model.Scripts.Count);
                    foreach (var script in model.Scripts)
                    {
                        binaryWriter.Write(script.Key);
                        binaryWriter.Write(script.Value);
                    }
                }
            }
        }
    }
}