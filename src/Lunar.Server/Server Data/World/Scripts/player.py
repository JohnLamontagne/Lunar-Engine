from Lunar.Core import *
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

    def OnDeath(self, player):
        player.Descriptor.Stats.Health = player.Descriptor.Stats.MaximumHealth
        player.SendChatMessage("You are dead!", ChatMessageType.Alert)
        player.JoinMap(player.Map)


BehaviorDefinition = PlayerBehaviorDefinition()
