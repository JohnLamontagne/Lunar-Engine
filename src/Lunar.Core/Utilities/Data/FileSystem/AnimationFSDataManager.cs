using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using System;
using System.IO;
using System.Text.Json;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    internal class AnimationFSDataManager : FSDataManager<BaseAnimation<IAnimationLayer<SpriteInfo>>>
    {
        private record AnimationLayerDto(int FrameWidth, int FrameHeight, int FrameTime, int LoopCount, string TexturePath);
        private record AnimationDto(string Name, AnimationLayerDto SubSurface, AnimationLayerDto Surface);

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ANIM_FILE_EXT);
        }

        public override BaseAnimation<IAnimationLayer<SpriteInfo>> Load(IDataManagerArguments arguments)
        {
            string json = File.ReadAllText(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ANIM_FILE_EXT);
            var dto = JsonSerializer.Deserialize<AnimationDto>(json, JsonOptions);

            var animation = new BaseAnimation<IAnimationLayer<SpriteInfo>>();
            animation.Name = dto.Name;
            animation.SubSurfaceAnimation.FrameWidth = dto.SubSurface.FrameWidth;
            animation.SubSurfaceAnimation.FrameHeight = dto.SubSurface.FrameHeight;
            animation.SubSurfaceAnimation.FrameTime = dto.SubSurface.FrameTime;
            animation.SubSurfaceAnimation.LoopCount = dto.SubSurface.LoopCount;
            animation.SubSurfaceAnimation.TexturePath = dto.SubSurface.TexturePath;
            animation.SurfaceAnimation.FrameWidth = dto.Surface.FrameWidth;
            animation.SurfaceAnimation.FrameHeight = dto.Surface.FrameHeight;
            animation.SurfaceAnimation.FrameTime = dto.Surface.FrameTime;
            animation.SurfaceAnimation.LoopCount = dto.Surface.LoopCount;
            animation.SurfaceAnimation.TexturePath = dto.Surface.TexturePath;

            return animation;
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            throw new NotImplementedException();
        }
    }
}
