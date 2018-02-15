/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lunar.Server.Utilities.Scripting;
using System;
using Lunar.Core.Utilities;

namespace Lunar.Server.World.BehaviorDefinition
{
    public class ActorBehaviorDefinition
    {
        /// <summary>
        /// Invoked upon actor death
        /// </summary>
        public ScriptAction OnDeath { get; set; }

        /// <summary>
        ///  Attacks the specified actor, returning the amount of damage delt.
        /// </summary>
        public ScriptFunction Attack { get; set; }

        /// <summary>
        /// Invoked upon being attacked by the specified actor
        /// </summary>
        public ScriptAction Attacked { get; set; }

        /// <summary>
        /// Invoked every actor Update() pass
        /// </summary>
        public ScriptAction Update { get; set; }

        /// <summary>
        /// Invoked when the actor is created
        /// </summary>
        public ScriptAction OnCreated { get; set; }

        public virtual event EventHandler<SubjectEventArgs> EventOccured;
    }
}