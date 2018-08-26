/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using System.IO;

namespace Lunar.Core.Content.Graphics
{
    public class AnimationDescriptor
    {
        public AnimationLayerDescriptor SurfaceAnimation { get; }

        public AnimationLayerDescriptor SubSurfaceAnimation { get; }

        public string Name { get; set; }

        private AnimationDescriptor()
        {
            this.SubSurfaceAnimation = new AnimationLayerDescriptor();
            this.SurfaceAnimation = new AnimationLayerDescriptor();
        }

        public static AnimationDescriptor Load(string filePath)
        {
            AnimationDescriptor animationDescription = new AnimationDescriptor();
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
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

        public static AnimationDescriptor Create()
        {
            AnimationDescriptor desc = new AnimationDescriptor();
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
                    binaryWriter.Write(this.Name);

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
