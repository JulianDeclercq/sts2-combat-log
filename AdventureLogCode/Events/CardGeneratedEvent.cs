using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AdventureLog.AdventureLogCode.Events;

public sealed record CardGeneratedEvent(
    string GeneratedCardName,
    CardModel? GeneratedCard,
    PileType? Pile,
    bool GeneratedByPlayer,
    ulong? OwnerNetId,
    string OwnerName,
    bool IsLocal,
    int TurnNumber,
    int OrderInTurn,
    int CombatNumber)
    : LogEvent(OwnerNetId, OwnerName, IsLocal, TurnNumber, OrderInTurn, CombatNumber);
