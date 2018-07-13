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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics.Effects
{
    public class Emitter
    {
        private List<Particle> _particles;
        private bool _emitting;
        private Texture2D[] _textures;

        protected Random Random;

        public Vector2 Position { get; set; }

        public int TotalParticles { get; set; }

        public Emitter(int totalParticles, params Texture2D[] textures)
        {
            _particles = new List<Particle>();
            _textures = textures;
            this.Random = new Random();

            this.TotalParticles = totalParticles;
        }

        public void Emit()
        {
            _particles.Clear();
            _emitting = true;
        }

        public void Stop()
        {
            _particles.Clear();
            _emitting = false;
        }

        protected virtual Particle GenerateNewParticle()
        {
            Texture2D texture = _textures[this.Random.Next(_textures.Length)];
            Vector2 position = this.Position;
            Vector2 velocity = new Vector2(1f * (float)(this.Random.NextDouble() * 2 - 1), 1f * (float)(this.Random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(this.Random.NextDouble() * 2 - 1);
            Color color = new Color((float)this.Random.NextDouble(), (float)this.Random.NextDouble(), (float)this.Random.NextDouble());
            float size = (float)this.Random.NextDouble();
            int ttl = 20 + this.Random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!_emitting) return;

            if (_particles.Count < this.TotalParticles)
            {
                int diff = this.TotalParticles - _particles.Count;

                for (int i = 0; i < diff; i++)
                {
                    _particles.Add(this.GenerateNewParticle());
                }
            }

            for (int i = 0; i < _particles.Count; i++)
            {
                _particles[i].Update(gameTime);

                if (!_particles[i].IsAlive)
                {
                    _particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!_emitting) return;

            for (int i = 0; i < _particles.Count; i++)
            {
                _particles[i].Draw(spriteBatch);
            }
        }
    }
}
