namespace AdventureLog.AdventureLogCode.Events;

public sealed record BlockGainedEvent(
    string ReceiverName,
    uint? ReceiverCombatId,
    int Amount,
    string? SourceCardName,
    ulong? OwnerNetId,
    string OwnerName,
    bool IsLocal,
    int TurnNumber,
    int OrderInTurn,
    int CombatNumber)
    : LogEvent(OwnerNetId, OwnerName, IsLocal, TurnNumber, OrderInTurn, CombatNumber);
