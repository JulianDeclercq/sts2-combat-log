using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AdventureLog.AdventureLogCode.UI.Rows;

internal static class TinyCardFactory
{
    private const string ScenePath = "res://scenes/cards/tiny_card.tscn";
    private static PackedScene? _scene;

    public static NTinyCard? Build(CardModel card, float iconSize, float scale = 0.4f)
    {
        _scene ??= GD.Load<PackedScene>(ScenePath);
        if (_scene is null) return null;

        var node = _scene.Instantiate<NTinyCard>();
        // CustomMinimumSize is load-bearing for AdventureLogPanel scroll compensation: it
        // gives GetCombinedMinimumSize a correct value synchronously, before SetCard runs
        // on the Ready signal (deferred per CLAUDE.md gotcha #8).
        node.CustomMinimumSize = new Vector2(iconSize, iconSize);
        node.Scale = new Vector2(scale, scale);
        node.Ready += () => node.SetCard(card);
        return node;
    }
}
