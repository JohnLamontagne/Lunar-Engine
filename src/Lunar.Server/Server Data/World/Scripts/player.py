import sys
import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
clr.AddReference('System')
import npc_common
from Lunar.Server.Utilities import *
from Lunar.Server.World.BehaviorDefinition import *


class PlayerBehaviorDefinition(ActorBehaviorDefinition):
    
    def __init__(self):
        return
    
    def Update(self, player, gameTime):
        return

    def OnCreated(self, player):
        print("Player behavior definition created...")

    def Attack(self, player, target):
        return 10

    def Attacked(self, player, attacker, damage_delt):
        player.Descriptor.Stats.Health -= damage_delt
        player.NetworkComponent.SendPlayerStats()

    def OnDeath(self, player):
        player.SendChatMessage("You are dead!", ChatMessageType.Alert)
        player.Description.Stats.Health = player.Description.Stats.MaximumHealth
        player.JointMap(player.Map)


BehaviorDefinition = PlayerBehaviorDefinition()
