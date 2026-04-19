using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace AdventureLog.AdventureLogCode.Patches;

/// <summary>
/// Logs potion use. Downstream damage / block / power events are already recorded
/// by their own patches; this row makes their source visible.
/// </summary>
[HarmonyPatch(typeof(CombatHistory), nameof(CombatHistory.PotionUsed))]
public static class PotionUsedPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState __0, PotionModel __1, Creature? __2)
    {
        try
        {
            var potion = __1;
            var target = __2;
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
