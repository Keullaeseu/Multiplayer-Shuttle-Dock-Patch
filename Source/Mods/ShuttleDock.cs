using HarmonyLib;
using MThings.ShuttleDock;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerShuttleDockPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Shuttle Dock by Salvador, Last Update: 4 Aug @ 4:22pm 2026
///     https://steamcommunity.com/workshop/filedetails/?id=3648642429
/// </summary>
[MpCompatFor("Salvador143.ShuttleDock")]
public class ShuttleDockPatch
{
    public ShuttleDockPatch(ModContentPack _content)
    {
        Log.Message("[Multiplayer Shuttle Dock Patch] initializing...");

        if (!MP.enabled)
        {
            Log.Error("[Multiplayer Shuttle Dock Patch] Multiplayer is disabled.");
            return;
        }

        var _toggleRoofMethodInfo = AccessTools.Method(typeof(Building_ShuttleDock), "ToggleRoof");
        if (_toggleRoofMethodInfo == null)
            Log.Error("[Multiplayer Shuttle Dock Patch] Could not find Building_ShuttleDock.ToggleRoof.");

        MP.RegisterSyncMethod(_toggleRoofMethodInfo);
        Log.Message("[Multiplayer Shuttle Dock Patch] initialized.");
    }
}