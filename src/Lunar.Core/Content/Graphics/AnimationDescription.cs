using System.IO;

namespace Lunar.Core.Content.Graphics
{
    public class AnimationDescription
    {
        public AnimationLayerDefinition SurfaceAnimation { get; }

        public AnimationLayerDefinition SubSurfaceAnimation { get; }

        private AnimationDescription()
        {
            this.SubSurfaceAnimation = new AnimationLayerDefinition();
            this.SurfaceAnimation = new AnimationLayerDefinition();
        }

        public static AnimationDescription Load(string filePath)
        {
            AnimationDescription animationDescription = new AnimationDescription();
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
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

        public static AnimationDescription Create()
        {
            AnimationDescription desc = new AnimationDescription();
            desc.SubSurfaceAnimation.TexturePath = "";
            desc.SurfaceAnimation.TexturePath = "";

            return desc;
        }

        public void Save(string filePath)
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(this.SubSurfaceAnimation.FrameWidth);
                    binaryWriter.Write(this.SubSurfaceAnimation.FrameHeight);
                    binaryWriter.Write(this.SubSurfaceAnimation.FrameTime);
                    binaryWriter.Write(this.SubSurfaceAnimation.LoopCount);
                    binaryWriter.Write(this.SubSurfaceAnimation.TexturePath);

                    binaryWriter.Write(this.SurfaceAnimation.FrameWidth);
                    binaryWriter.Write(this.SurfaceAnimation.FrameHeight);
                    binaryWriter.Write(this.SurfaceAnimation.FrameTime);
                    binaryWriter.Write(this.SurfaceAnimation.LoopCount );
                    binaryWriter.Write(this.SurfaceAnimation.TexturePath);
                }
            }
        }
    }
}
