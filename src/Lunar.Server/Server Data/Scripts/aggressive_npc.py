import sys
import clr
clr.AddReference('Lunar.Core')
clr.AddReference('Lunar.Server')
clr.AddReference('System')
import npc_common


def update(args):
	acquireTarget(args.Invoker, args[0])

def on_created(args):
	randomWalkTmr = GameTimer(1000)
	GameTimerManager.Register("randomWalkTmr", randomWalkTmr)

def attack(args):
	return 10
