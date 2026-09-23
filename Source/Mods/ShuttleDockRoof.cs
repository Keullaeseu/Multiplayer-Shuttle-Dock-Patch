using HarmonyLib;
using MThings.ShuttleDock;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerShuttleDockPatch.Source.Mods;

/// <summary>Syncs <see cref="Building_ShuttleDock" /> roof state ("Open/Close dock", "Use ship roof").</summary>
internal static class ShuttleDockRoof
{
    internal static void Patch()
    {
        // Building_ShuttleDock.ToggleRoof() - private, called from GetGizmos "Open/Close dock" action.
        // Syncing the method directly covers the gizmo (MP intercepts the call).
        // No need to also sync GetGizmos lambda 0, it just calls ToggleRoof (that would be a duplicate).
        var _toggleRoof = AccessTools.Method(typeof(Building_ShuttleDock), "ToggleRoof");
        if (_toggleRoof == null)
            Log.Error($"{ShuttleDock.LogPrefix} Could not find Building_ShuttleDock.ToggleRoof.");
        else
            MP.RegisterSyncMethod(_toggleRoof);

        // Building_ShuttleDock.GetGizmos lambda 2 - "Use ship roof" Command_Toggle.toggleAction.
        // Verified via dnlib on ShuttleDock 1.0.6 (DisplayClass22_0): b__0 = roof toggle wrapper,
        // b__1 = isActive getter (do NOT sync), b__2 = useShipRoof toggle that directly mutates
        // useShipRoof + roof grid + NotifyLinkedLamps. No named method exists, so the lambda
        // itself MUST be synced. This is not a duplicate of anything.
        try
        {
            MpCompat.RegisterLambdaMethod(typeof(Building_ShuttleDock), "GetGizmos", 2);
        }
        catch (Exception _exception)
        {
            Log.Error(
                $"{ShuttleDock.LogPrefix} Failed to sync Building_ShuttleDock useShipRoof toggle: {_exception}");
        }
    }
}