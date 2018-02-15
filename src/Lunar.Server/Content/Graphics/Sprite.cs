/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lunar.Core.Content.Graphics;

namespace Lunar.Server.Content.Graphics
{
    public class Sprite
    {
        public Transform Transform { get; set; }

        public string TextureName { get; set; }

        public Sprite(string textureName)
        {
            this.TextureName = textureName;
            this.Transform = new Transform();
        }
    }
}