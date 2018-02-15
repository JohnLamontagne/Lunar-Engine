/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Utilities;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Server.World.Structure;

namespace Lunar.Server.World.Actors
{
    public interface IActor : ISubject
    {
        long UniqueID { get; }

        string Name { get; }

        float Speed { get; set; }

        int Level { get; set; }

        int Health { get; set; }

        int MaximumHealth { get; set; }

        bool Attackable { get; }

        Vector Position { get; }

        Layer Layer { get; set; }

        Rect CollisionBounds { get; }

        ActorBehaviorDefinition BehaviorDefinition { get; }

        IActor Target { get; set; }

        void Update(GameTime gameTime);

        void WarpTo(Vector position);

        void OnAttacked(IActor attacker, int damageDelt);
    }
}