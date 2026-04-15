using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace CombatLog.CombatLogCode.Patches;

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.GainEnergy))]
public static class EnergyDeltaPatch
{
    [HarmonyPostfix]
    public static void Postfix(decimal __0, Player __1)
    {
        try
        {
            var delta = (int)__0;
            if (delta <= 0) return;

            var ownerNetId = __1?.NetId;
            OwnerResolver.Resolve(ownerNetId, out var ownerName, out var isLocal);

            var playerCombatId = __1?.Creature?.CombatId;

            Texture2D? icon = null;
            try
            {
                var cardPool = __1?.Character?.CardPool;
                if (cardPool is not null)
                {
                    var path = EnergyIconHelper.GetPath(cardPool);
                    icon = PreloadManager.Cache.GetTexture2D(path);
                }
            }
            catch (Exception iconEx)
            {
                GD.PrintErr($"[CombatLog] Error loading energy icon: {iconEx.Message}");
            }

            CombatLogTracker.RecordEnergyDelta(
                delta, icon, playerCombatId,
                ownerNetId, ownerName, isLocal);
        }
        catch (Exception e)
        {
            GD.PrintErr($"[CombatLog] Error recording energy delta: {e.Message}");
        }
    }
}
