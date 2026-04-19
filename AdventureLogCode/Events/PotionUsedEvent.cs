using Godot;
using MegaCrit.Sts2.Core.Models;

namespace AdventureLog.AdventureLogCode.Events;

public sealed record PotionUsedEvent(
    string PotionTitle,
    PotionModel? Potion,
    Texture2D? Icon,
    string? TargetName,
    uint? TargetCombatId,
    ulong? OwnerNetId,
    string OwnerName,
    bool IsLocal,
    int TurnNumber,
    int OrderInTurn,
    int CombatNumber)
    : LogEvent(OwnerNetId, OwnerName, IsLocal, TurnNumber, OrderInTurn, CombatNumber);
