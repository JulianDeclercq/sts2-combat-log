using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace CombatLog.CombatLogCode.Patches;

/// <summary>
/// Patches CombatManager.SetupPlayerTurn to track turn progression (player turns only),
/// and StartCombatInternal to reset turn counter per combat.
/// </summary>
[HarmonyPatch(typeof(CombatManager), "SetupPlayerTurn")]
public static class TurnStartPatch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        CombatLogTracker.OnNewTurn();
    }
}

[HarmonyPatch(typeof(CombatManager), "StartCombatInternal")]
public static class CombatStartPatch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        CombatLogTracker.OnCombatStart();
    }
}
