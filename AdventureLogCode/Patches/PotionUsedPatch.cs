using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AdventureLog.AdventureLogCode.Patches;

/// <summary>
/// Logs potion use. Patches PotionModel.OnUseWrapper as a Prefix so the
/// PotionUsedEvent is recorded BEFORE the OnUse effects run — letting the
/// render layer consume subsequent power/block/damage rows as children of
/// the potion (same nesting pattern as card play).
/// </summary>
[HarmonyPatch(typeof(PotionModel), nameof(PotionModel.OnUseWrapper))]
public static class PotionUsedPatch
{
    [HarmonyPrefix]
    public static void Prefix(PotionModel __instance, PlayerChoiceContext __0, Creature? __1)
    {
        try
        {
            var potion = __instance;
            var target = __1;
            if (potion is null) return;

            var title = potion.Title?.GetFormattedText() ?? potion.Id?.Entry ?? potion.GetType().Name;
            var icon = potion.Image;
            var ownerNetId = potion.Owner?.NetId;
            OwnerResolver.Resolve(ownerNetId, out var ownerName, out var isLocal);

            AdventureLogTracker.RecordPotionUsed(
                title, potion, icon, target?.Name, target?.CombatId,
                ownerNetId, ownerName, isLocal);
        }
        catch (Exception e)
        {
            GD.PrintErr($"[AdventureLog] Error recording potion used: {e.Message}");
        }
    }
}
