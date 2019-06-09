#!/usr/bin/python
# -*- coding: utf-8 -*-
import random
import npc_common
from Lunar.Core import *
from Lunar.Core.Utilities.Data import *
from Lunar.Core.Utilities.Logic import *
from Lunar.Server.Utilities import *
from Lunar.Server.World.BehaviorDefinition import *
from Lunar.Server.World.Actors import *


class CombatNPCState(IActorState[NPC]):
    def __init__(self):
        return

    def OnEnter(self, npc):
        nextAttackTimer = GameTimer(2000)
        npc.GameTimers.Register('nextAttackTimer', nextAttackTimer)

    def OnExit(self, npc):
        return

    def Update(self, gameTime, npc):
        if not npc.Target or not npc.Aggrevated or not npc.Target.Attackable:
            return IdleState()

        if npc.GameTimers.Get('nextAttackTimer').Finished:
            if npc.WithinAttackingRangeOf(npc.Target):
                npc.Behavior.Attack(npc, npc.Target)
                npc.GameTimers.Get('nextAttackTimer').Reset()
            else:
                npc.GoTo(npc.Target)
                return MovingState(self)

        return self


class IdleState(IActorState[NPC]):
    def __init__(self):
        return

    def OnEnter(self, npc):
        return

    def OnExit(self, npc):
        return

    def Update(self, gameTime, npc):
        target = npc.FindPlayerTarget()
        
        if target:
            npc.Target = target
            npc.Aggrevated = True
            return CombatNPCState()
        elif npc.GameTimers.Get('randomWalkTmr').Finished:
            return WanderState()
        else:
            return self


class MovingState(IActorState[NPC]):
    def __init__(self, return_state):
        self.return_state = return_state

    def OnEnter(self, npc):
        return

    def OnExit(self, npc):
        return

    def Update(self, gameTime, npc):
        if not npc.Moving:
            return self.return_state
        else:
            # continue to check for combat opportunities while moving
            target = npc.FindPlayerTarget()
            if target:
                npc.Target = target
                npc.Aggrevated = True
                print("combat")
                return CombatNPCState()
            else:
                return self


class WanderState(IActorState[NPC]):
    def __init__(self):
        return

    def OnEnter(self, npc):
        return

    def OnExit(self, npc):
        return

    def Update(self, gameTime, npc):
        if npc.GameTimers.Get('randomWalkTmr').Finished:
            npc.GameTimers.Get('randomWalkTmr').Reset()
            direction = (-1 if random.random() < .5 else 1)
            randomX = random.random() * (npc.Descriptor.MaxRoam.X * EngineConstants.TILE_WIDTH) * direction
            randomY = random.random() * (npc.Descriptor.MaxRoam.Y * EngineConstants.TILE_HEIGHT) * direction
            dest = Vector(randomX, randomY)
            print("Moving to " + str(dest))
            npc.GoTo(dest)
            return MovingState(self)  # will return to this state when it is finished
        else:
            print("returning to idle")
            return IdleState(npc)


class AggressiveNPCBehaviorDefinition(ActorBehaviorDefinition):
    def __init__(self):
        return 0

    def Update(self, npc, gameTime):
        return

    def OnCreated(self, npc):
        randomWalkTmr = GameTimer(500)
        npc.GameTimers.Register('randomWalkTmr', randomWalkTmr)
        npc.StateMachine.Start(IdleState())

    def Attack(self, npc, target):
        damage = 5
        target.OnAttacked(npc, damage)

    def Attacked(self, npc, attacker, damage_delt):
        print 'Not implemented'


# Create an object of our AggressiveNPCBehaviorDefinition
# and assign it to BehaviorDefinition. This is used by the
# server to hook in our behavior.

BehaviorDefinition = AggressiveNPCBehaviorDefinition()
