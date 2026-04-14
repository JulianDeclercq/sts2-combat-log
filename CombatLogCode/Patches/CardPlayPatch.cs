using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;

namespace CombatLog.CombatLogCode.Patches;

/// <summary>
/// Patches CardModel.OnPlayWrapper to record each card as it's played.
/// OnPlayWrapper signature: (PlayerChoiceContext, Creature? target, bool, ResourceInfo, bool)
/// </summary>
[HarmonyPatch(typeof(CardModel), "OnPlayWrapper")]
public static class CardPlayPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance, Creature? __1)
    {
        try
        {
            var cardName = __instance.Title ?? __instance.GetType().Name;
            ResolveOwner(__instance, out var ownerNetId, out var ownerName, out var isLocal);

            var targetName = __1?.Name ?? "";
            var targetCombatId = __1?.CombatId;
            var playerCombatId = __instance.Owner?.Creature?.CombatId;

            CombatLogTracker.RecordCardPlay(
                cardName, __instance,
                ownerNetId, ownerName, isLocal,
                targetName, targetCombatId, playerCombatId);
        }
        catch (Exception e)
        {
            GD.PrintErr($"[CombatLog] Error recording card play: {e.Message}");
        }
    }

    private static void ResolveOwner(CardModel card, out ulong? netId, out string name, out bool isLocal)
    {
        netId = null;
        name = "";
        isLocal = true;

        try
        {
            if (card.Owner is null) return;
            netId = card.Owner.NetId;

            var netService = RunManager.Instance?.NetService;
            if (netService is null) return;

            var resolved = PlatformUtil.GetPlayerName(netService.Platform, card.Owner.NetId);
            if (!string.IsNullOrEmpty(resolved) && !ulong.TryParse(resolved, out _))
                name = resolved;

            try
            {
                var localNetId = Traverse.Create(netService).Property("LocalPlayer").Field("NetId").GetValue<ulong>();
                isLocal = localNetId == card.Owner.NetId;
            }
            catch
            {
                // LocalPlayer not resolvable — keep default (treat as local to match solo behavior)
            }
        }
        catch { }
    }
}
