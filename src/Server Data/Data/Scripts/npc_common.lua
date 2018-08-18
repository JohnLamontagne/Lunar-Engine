import('Lunar.Core.World.Actor')

function acquireTarget(npc, gameTime)
	target = npc:FindPlayerTarget()

	if (target) then
		npc.Target = target
		npc.Aggrevated = true
	elseif (not npc.State == ActorStates.Moving) then -- No target to find, let's just try finding a random place to wander.
		randomWalkTmr = GameTimerManager.Instance:GetTimer("randomWalkTmr" .. npc:GetHashCode())

		if (randomWalkTmr.Finished) then
			randomX = math.random() * (npc.MaxRoam.X * Constants.TILE_SIZE)
			randomY = math.random() * (npc.MaxRoam.Y * Constants.TILE_SIZE)

			if (math.random() < .5) then
				signX = -1
			else
				signX = 1
			end

			if (math.random() < .5) then
				signY = -1
			else
				signY = 1
			end

			npc:GoTo(Vector(randomX * signX, randomY * signY))

			randomWalkTmr:Reset()
		end
	end
end
