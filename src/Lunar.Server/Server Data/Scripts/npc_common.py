import clr
clr.AddReference('Lunar.Core')
import math

def acquireTarget(npc, gameTime):
	target = npc.FindPlayerTarget()

	if target:
		npc.Target = target
		npc.Aggrevated = true
	elif not npc.State == ActorStates.Moving: # No target to find, let's just try finding a random place to wander.
		randomWalkTmr = GameTimerManager.Instance.GetTimer("randomWalkTmr" + npc.GetHashCode())

		if randomWalkTmr.Finished:
			randomX = math.random() * (npc.MaxRoam.X * Constants.TILE_SIZE)
			randomY = math.random() * (npc.MaxRoam.Y * Constants.TILE_SIZE)
			
			signx = -1 if math.random() < .5 else 1
			signy = -1 if math.random() < .5 else 1

			npc.GoTo(Vector(randomX * signX, randomY * signY))

			randomWalkTmr.Reset()
