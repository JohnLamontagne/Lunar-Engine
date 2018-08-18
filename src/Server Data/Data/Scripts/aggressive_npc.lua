import('Lunar.Core')
import('Lunar.Core.Utilities')
import('Lunar.Core.Utilities.Data')
import('Lunar.Core.Utilities.Logic')
import("System")
import("Lunar.Server")
import("Lunar.Server.Utilities")
import("Lunar.Server.World.Actors")
import("Lunar.Server.World.BehaviorDefinition")
import("Lunar.Server.Utilities.Scripting")

require("Data.Scripts.npc_common")

function Update(args)
	acquireTarget(args.Invoker, args[0])
end

function OnCreated(args)
	randomWalkTmr = GameTimer(1000)

	GameTimerManager.Instance:Register("randomWalkTmr", randomWalkTmr)
end

function Attack(args)
	return 10
end
