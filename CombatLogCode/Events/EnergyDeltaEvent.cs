using Godot;

namespace CombatLog.CombatLogCode.Events;

public sealed record EnergyDeltaEvent(
    int Delta,
    Texture2D? Icon,
    uint? PlayerCombatId,
    ulong? OwnerNetId,
    string OwnerName,
    bool IsLocal,
    int TurnNumber,
    int OrderInTurn,
    int CombatNumber)
    : LogEvent(OwnerNetId, OwnerName, IsLocal, TurnNumber, OrderInTurn, CombatNumber);
