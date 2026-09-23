using HarmonyLib;
using MThings.ShuttleDock;
using Multiplayer.API;
using Multiplayer.Compat;
using UnityEngine;
using Verse;

namespace MultiplayerShuttleDockPatch.Source.Mods;

/// <summary>
///     Fixes <see cref="Dialog_DockIndicatorColorPicker" /> saving for multiplayer.
///     The mod's SaveColor reads back <c>GlowColor</c> after calling base.SaveColor, but in MP
///     the base call (via the vanilla-synced SetGlowColorInternal) is queued, so the read-back
///     still holds the stale pre-save value and the picked color is lost.
/// </summary>
internal static class ShuttleDockColorDialog
{
    private static AccessTools.FieldRef<object, CompDockLink> dialogOwnerField;
    private static AccessTools.FieldRef<object, bool> dialogEditingOpenField;

    internal static void Patch()
    {
        dialogOwnerField = AccessTools.FieldRefAccess<CompDockLink>(typeof(Dialog_DockIndicatorColorPicker), "owner");
        dialogEditingOpenField =
            AccessTools.FieldRefAccess<bool>(typeof(Dialog_DockIndicatorColorPicker), "editingOpenColor");
        if (dialogOwnerField == null || dialogEditingOpenField == null)
        {
            Log.Error($"{ShuttleDock.LogPrefix} Could not resolve Dialog_DockIndicatorColorPicker fields.");
            return;
        }

        var _saveColor = AccessTools.DeclaredMethod(typeof(Dialog_DockIndicatorColorPicker), "SaveColor");
        if (_saveColor == null)
        {
            Log.Error($"{ShuttleDock.LogPrefix} Could not find Dialog_DockIndicatorColorPicker.SaveColor.");
            return;
        }

        MpCompat.harmony.Patch(_saveColor,
            new HarmonyMethod(typeof(ShuttleDockColorDialog), nameof(PreSaveColor)));
        MP.RegisterSyncMethod(typeof(ShuttleDockColorDialog), nameof(SyncedSetDockColor));
    }

    // Runs on the clicking client when Accept is pressed in the dock color picker.
    // Returns false in MP to skip the original (stale-read) implementation.
    private static bool PreSaveColor(Dialog_DockIndicatorColorPicker __instance, Color color)
    {
        if (!MP.IsInMultiplayer || MP.IsExecutingSyncCommand)
            return true;

        var _owner = dialogOwnerField(__instance);
        if (_owner == null)
            return true;

        SyncedSetDockColor(_owner, dialogEditingOpenField(__instance), color);
        return false;
    }

    // Runs on ALL clients. `picked` is passed by value so it is exact everywhere.
    // Uses an exact Color -> ColorInt conversion (deterministic, no dependency on the
    // potentially diverged current glow), then reuses the mod's own logic which sets
    // open/closed + colorsInitialized and refreshes the lamp glow via Notify.
    private static void SyncedSetDockColor(CompDockLink _comp, bool _isOpenColor, Color _picked)
    {
        if (_comp == null)
            return;
        _comp.SetDockIndicatorColor(_isOpenColor, new ColorInt(_picked));
    }
}