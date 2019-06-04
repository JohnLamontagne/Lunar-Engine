import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
import math
from Lunar.Server import *
from Lunar.Core.World.Actor import *


def acquire_target(npc, gameTime):
	target = npc.FindPlayerTarget()

	if target:
		npc.Target = target
		npc.Aggrevated = True
	elif not npc.State == ActorStates.Moving: # No target to find, let's just try finding a random place to wander.
		randomWalkTmr = npc.GameTimerManager.GetTimer("randomWalkTmr" + str(npc.GetHashCode()))

		if randomWalkTmr and randomWalkTmr.Finished:
			random_walk(npc)
			randomWalkTmr.Reset()
			
			
def random_walk(npc):
	randomX = math.random() * (npc.MaxRoam.X * Constants.TILE_SIZE)
	randomY = math.random() * (npc.MaxRoam.Y * Constants.TILE_SIZE)
	
	signx = -1 if math.random() < .5 else 1
	signy = -1 if math.random() < .5 else 1

	npc.GoTo(Vector(randomX * signX, randomY * signY))

	
