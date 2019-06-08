import sys
import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
clr.AddReference('System')
import npc_common 
from Lunar.Server.Utilities import *
from Lunar.Server.World.BehaviorDefinition import *
from Lunar.Server.World.Actors import *
import random

class CombatNPCState(IActorState[NPC]):
	def __init__(self):
		return
		
	def OnEnter(self, npc):
		nextAttackTimer = GameTimer(1000)
		npc.GameTimers.Register("nextAttackTimer", nextAttackTimer)
		
	def OnExit(self, npc):
		return
		
	def Update(self, gameTime, npc):
		if not npc.Target or not npc.Aggrevated or not npc.Target.Attackable \
			or not npc.Target.IsAlive or npc.Target.InLoadingScreen:
				return IdleState(npc)
		
		if npc.GameTimers.Get("nextAttackTimer").Finished:
			deltaPos = Helpers.Abs(npc.Descriptor.Position - npc.Target.Descriptor.Position)
			
			if deltaPos.X.IsWithin(0, npc.Descriptor.AttackRange) and \
				deltaPos.Y.IsWithin(0, npc.Descriptor.AttackRange):
					npc.Behavior.Attack(npc, npc.Target)
					npc.GameTimers.Get("nextAttackTimer").Reset()
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
		
		print(target)
		
		if target:
			npc.Target = target
			npc.Aggrevated = True
			return CombatNPCState(npc)
		elif npc.GameTimers.Get("randomWalkTmr").Finished:
			print("wandering...")
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
		else: # continue to check for combat opportunities while moving
			target = npc.FindPlayerTarget()
			if target:
				npc.Target = target
				npc.Aggrevated = True
				return CombatNPCState()
		 

class WanderState(IActorState[NPC]):	
	def __init__(self):
		return
		
	def OnEnter(self, npc):
		npc.GameTimers.Get("randomWalkTmr").Reset()
		
	def OnExit(self, npc):
		return
		
	def Update(self, gameTime, npc):		
		if npc.GameTimers.Get("randomWalkTmr").Finished:				
			direction = -1 if random.random() < .5 else 1
			randomX = random.random() * (npc.MaxRoam.X * Constants.TILE_SIZE) * direction
			randomY = random.random() * (npc.MaxRoam.Y * Constants.TILE_SIZE) * direction
			npc.GoTo(Vector(randomX, randomY))
			
			return MovingState(self) # will return to this state when it is finished
		else:
			return IdleState(npc)
					

class AggressiveNPCBehaviorDefinition(ActorBehaviorDefinition):
	def __init__(self):
		return 0
	
	def Update(self, npc, gameTime):
		return 
		
	def OnCreated(self, npc):
		randomWalkTmr = GameTimer(500)
		npc.GameTimers.Register("randomWalkTmr", randomWalkTmr)
		npc.StateMachine.Start(IdleState())

	def Attack(self, npc, target):
		damage = 10
		target.OnAttacked(npc, damage)
		
	def Attacked(self, npc, attacker, damage_delt):
		print("Not implemented")
		
# Create an object of our AggressiveNPCBehaviorDefinition
# and assign it to BehaviorDefinition. This is used by the
# server to hook in our behavior.
BehaviorDefinition = AggressiveNPCBehaviorDefinition()
