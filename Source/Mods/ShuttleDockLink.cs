using HarmonyLib;
using MThings.ShuttleDock;
using Multiplayer.API;
using Verse;

namespace MultiplayerShuttleDockPatch.Source.Mods;

/// <summary>Syncs <see cref="CompDockLink" /> lamp-dock linking and indicator colors.</summary>
internal static class ShuttleDockLink
{
    internal static void Patch()
    {
        // CompDockLink "Connect to nearby dock" / "Disconnect from dock" gizmos call these
        // private methods. Syncing them directly covers the gizmos, no need to also sync
        // CompGetGizmosExtra lambda 0 (that would be a duplicate).
        var _tryConnect = AccessTools.Method(typeof(CompDockLink), "TryConnectNearestDock");
        if (_tryConnect == null)
            Log.Error($"{ShuttleDock.LogPrefix} Could not find CompDockLink.TryConnectNearestDock.");
        else
            MP.RegisterSyncMethod(_tryConnect);

        var _disconnect = AccessTools.Method(typeof(CompDockLink), "Disconnect");
        if (_disconnect == null)
            Log.Error($"{ShuttleDock.LogPrefix} Could not find CompDockLink.Disconnect.");
        else
            MP.RegisterSyncMethod(_disconnect);

        // "Set dock OPEN/CLOSED color" gizmos (see MThings_ShuttleDock.xml: SetOpenColorLabel /
        // SetClosedColorLabel) open Dialog_DockIndicatorColorPicker. Its
        // SaveColor(Color) calls CompDockLink.SetDockIndicatorColor(bool, ColorInt).
        // Sync ONLY the comp method (ThingComp target serializes via parent Thing, same
        // pattern as vanilla MP syncing CompGlower.SetGlowColorInternal).
        // Do NOT sync Dialog_DockIndicatorColorPicker.SaveColor itself: dialogs/windows
        // have no MP sync worker, so MP throws "No writer for type Dialog_...".
        var _setColor = AccessTools.Method(typeof(CompDockLink), "SetDockIndicatorColor");
        if (_setColor == null)
            Log.Error($"{ShuttleDock.LogPrefix} Could not find CompDockLink.SetDockIndicatorColor.");
        else
            MP.RegisterSyncMethod(_setColor);
    }
}