using MegaCrit.Sts2.Core.Models;

namespace AdventureLog.AdventureLogCode.Events;

public sealed record CardPlayEvent(
    string CardName,
    CardModel? Card,
    string TargetName,
    uint? TargetCombatId,
    uint? PlayerCombatId,
    ulong? OwnerNetId,
    string OwnerName,
    bool IsLocal,
    int TurnNumber,
    int OrderInTurn,
    int CombatNumber,
    int? XValue = null)
    : LogEvent(OwnerNetId, OwnerName, IsLocal, TurnNumber, OrderInTurn, CombatNumber);
