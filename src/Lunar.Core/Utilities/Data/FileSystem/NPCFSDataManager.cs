using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;
using System.IO;
using System.Text.Json;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class NPCFSDataManager : FSDataManager<NPCModel>
    {
        private record VectorDto(float X, float Y);
        private record RectDto(int X, int Y, int Width, int Height);
        private record StatsDto(int Strength, int Defense, int Dexterity, int Vitality, int Intelligence);
        private record NpcDto(
            string Name,
            int Level,
            float Speed,
            StatsDto Stats,
            RectDto CollisionBounds,
            int AggresiveRange,
            string TexturePath,
            VectorDto MaxRoam,
            VectorDto FrameSize,
            VectorDto Reach,
            string BehaviorKey,
            string Dialogue,
            string DialogueBranch
        );

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly Logger _logger;

        public NPCFSDataManager(Logger logger)
        {
            _logger = logger;
        }

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.NPC_FILE_EXT);
        }

        public override NPCModel Load(IDataManagerArguments arguments)
        {
            try
            {
                var npcArguments = (arguments as ContentFileDataLoaderArguments);
                string json = File.ReadAllText(this.RootPath + npcArguments.FileName + EngineConstants.NPC_FILE_EXT);
                var dto = JsonSerializer.Deserialize<NpcDto>(json, JsonOptions);

                var desc = NPCModel.Create(npcArguments.FileName);
                desc.Name = dto.Name;
                desc.Level = dto.Level;
                desc.Speed = dto.Speed;
                desc.Stats = new Stats()
                {
                    Strength = dto.Stats.Strength,
                    Defense = dto.Stats.Defense,
                    Dexterity = dto.Stats.Dexterity,
                    Vitality = dto.Stats.Vitality,
                    Intelligence = dto.Stats.Intelligence,
                };
                desc.CollisionBounds = new Rect(dto.CollisionBounds.X, dto.CollisionBounds.Y, dto.CollisionBounds.Width, dto.CollisionBounds.Height);
                desc.AggresiveRange = dto.AggresiveRange;
                desc.TexturePath = dto.TexturePath;
                desc.MaxRoam = new Vector(dto.MaxRoam.X, dto.MaxRoam.Y);
                desc.FrameSize = new Vector(dto.FrameSize.X, dto.FrameSize.Y);
                desc.Reach = new Vector(dto.Reach.X, dto.Reach.Y);
                desc.BehaviorKey = dto.BehaviorKey;
                desc.Dialogue = dto.Dialogue;
                desc.DialogueBranch = dto.DialogueBranch;

                return desc;
            }
            catch (System.IO.IOException exception)
            {
                _logger.LogEvent("Unable to load NPC. " + exception.Message, LogTypes.ERROR, exception);
                return null;
            }
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            var npcDesc = (NPCModel)descriptor;
            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.NPC_FILE_EXT;

            var dto = new NpcDto(
                npcDesc.Name,
                npcDesc.Level,
                npcDesc.Speed,
                new StatsDto(npcDesc.Stats.Strength, npcDesc.Stats.Defense, npcDesc.Stats.Dexterity, npcDesc.Stats.Vitality, npcDesc.Stats.Intelligence),
                new RectDto(npcDesc.CollisionBounds.X, npcDesc.CollisionBounds.Y, npcDesc.CollisionBounds.Width, npcDesc.CollisionBounds.Height),
                npcDesc.AggresiveRange,
                npcDesc.TexturePath,
                new VectorDto(npcDesc.MaxRoam.X, npcDesc.MaxRoam.Y),
                new VectorDto(npcDesc.FrameSize.X, npcDesc.FrameSize.Y),
                new VectorDto(npcDesc.Reach.X, npcDesc.Reach.Y),
                npcDesc.BehaviorKey ?? "",
                npcDesc.Dialogue ?? "",
                npcDesc.DialogueBranch ?? ""
            );

            File.WriteAllText(filePath, JsonSerializer.Serialize(dto, JsonOptions));
        }
    }
}
