namespace Lunar.Editor
{
    public static class Constants
    {
        public const int NEW_MAP_X = 35;
        public const int NEW_MAP_Y = 35;

        public const string DEFAULT_PY_ACTOR_BEHAVIOR = @"import sys
import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
clr.AddReference('System')
import npc_common
from Lunar.Server.Utilities import *
from Lunar.Server.World.BehaviorDefinition import *


class AggressiveNPCBehaviorDefinition(ActorBehaviorDefinition):
	def __init__(self):
		print('not implemented')
	
	def Update(self, npc, gameTime):
		print('not implemented')

	def OnCreated(self, npc):
		print('not implemented')
		

	def Attack(self, npc, target):
		print('not implemented')
        return 0
		
	def Attacked(self, npc, attacker, damage_delt):
		print('not implemented')

# Create an object of our AggressiveNPCBehaviorDefinition
# and assign it to BehaviorDefinition. This is used by the
# server to hook in our behavior.
BehaviorDefinition = AggressiveNPCBehaviorDefinition()";
    }
}
