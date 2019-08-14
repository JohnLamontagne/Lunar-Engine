using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    internal class AnimationFSDataManager : FSDataManager<BaseAnimation<IAnimationLayer<SpriteInfo>>>
    {
        public override bool Exists(IDataManagerArguments arguments)
        {
            throw new NotImplementedException();
        }

        public override BaseAnimation<IAnimationLayer<SpriteInfo>> Load(IDataManagerArguments arguments)
        {
            var animationDescription = new BaseAnimation<IAnimationLayer<SpriteInfo>>();
            using (FileStream fileStream = File.Open(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    animationDescription.Name = binaryReader.ReadString();
                    animationDescription.SubSurfaceAnimation.FrameWidth = binaryReader.ReadInt32();
                    animationDescription.SubSurfaceAnimation.FrameHeight = binaryReader.ReadInt32();
                    animationDescription.SubSurfaceAnimation.FrameTime = binaryReader.ReadInt32();
                    animationDescription.SubSurfaceAnimation.LoopCount = binaryReader.ReadInt32();
                    animationDescription.SubSurfaceAnimation.TexturePath = binaryReader.ReadString();

                    animationDescription.SurfaceAnimation.FrameWidth = binaryReader.ReadInt32();
                    animationDescription.SurfaceAnimation.FrameHeight = binaryReader.ReadInt32();
                    animationDescription.SurfaceAnimation.FrameTime = binaryReader.ReadInt32();
                    animationDescription.SurfaceAnimation.LoopCount = binaryReader.ReadInt32();
                    animationDescription.SurfaceAnimation.TexturePath = binaryReader.ReadString();
                }
            }

            return animationDescription;
        }

        public override void Save(IContentDescriptor descriptor, IDataManagerArguments arguments)
        {
            throw new NotImplementedException();
        }
    }
}