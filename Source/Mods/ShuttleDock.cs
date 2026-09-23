using Multiplayer.Compat;
using Verse;

namespace MultiplayerShuttleDockPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Shuttle Dock by Salvador, Last Update: 4 Aug @ 4:22pm 2026
///     https://steamcommunity.com/workshop/filedetails/?id=3648642429
/// </summary>
[MpCompatFor("Salvador143.ShuttleDock")]
public class ShuttleDock
{
    internal const string LogPrefix = "[Multiplayer Shuttle Dock Patch]";

    public ShuttleDock(ModContentPack _content)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        Log.Message($"{LogPrefix} Initializing...");

        ShuttleDockRoof.Patch();
        ShuttleDockLink.Patch();
        ShuttleDockColorDialog.Patch();

        Log.Message($"{LogPrefix} Initialized.");
    }
}