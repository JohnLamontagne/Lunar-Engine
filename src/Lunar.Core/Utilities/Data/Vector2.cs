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
using System;
using Microsoft.Xna.Framework;

namespace Lunar.Core.Utilities.Data
{
    public static class Vector2Helper
    {
        public static Vector2 FromString(this Vector2 vec, string value)
        {
            try
            {
                value = value.TrimStart('{').TrimEnd('}');

                var components = value.Split(' ');

                // Remove the X:/Y:
                components[0] = components[0].Remove(0, 2);
                components[1] = components[1].Remove(0, 2);

                return new Vector2(float.Parse(components[0]), float.Parse(components[1]));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid string: only strings that have the {X:a Y:b} format can be translated into a vector!", ex);
            }
        }
    }
}