/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

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
    public class BaseAnimation<T> : IAnimation<T> where T : IAnimationLayer<SpriteInfo>
    {
        public T SurfaceAnimation { get; protected set; }

        public T SubSurfaceAnimation { get; protected set; }

        public string Name { get; set; }

        protected BaseAnimation(string name)
        {
            this.Name = name;
        }

        public BaseAnimation()
        {
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
                    binaryWriter.Write(this.SurfaceAnimation.LoopCount);
                    binaryWriter.Write(this.SurfaceAnimation.TexturePath);
                }
            }
        }

        public static BaseAnimation<T> Create()
        {
            var desc = new BaseAnimation<T>("blankanimation");

            return desc;
        }
    }
}