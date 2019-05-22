import sys
import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
clr.AddReference('System')
import npc_common
from Lunar.Server.Utilities import *
from Lunar.Server.World.BehaviorDefinition import *


class AggressiveNPCBehaviorDefinition(ActorBehaviorDefinition):
	def __init__(self):
		return 0
	
	def Update(self, npc, gameTime):
		npc_common.acquire_target(npc, gameTime)

	def OnCreated(self, npc):
		print("NPC " + str(npc.Descriptor.Name) + " aggressive behavior handler created...")
		randomWalkTmr = GameTimer(500)
		npc.GameTimerManager.Register("randomWalkTmr" + str(npc.GetHashCode()), randomWalkTmr)
		

	def Attack(self, npc, target):
		return 10
		
	def Attacked(self, npc, attacker, damage_delt):
		print("Not implemented")
		
# Create an object of our AggressiveNPCBehaviorDefinition
# and assign it to BehaviorDefinition. This is used by the
# server to hook in our behavior.
BehaviorDefinition = AggressiveNPCBehaviorDefinition()
