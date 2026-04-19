using AdventureLog.AdventureLogCode.Events;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace AdventureLog.AdventureLogCode.UI.Rows;

public partial class CardGeneratedRow : HBoxContainer
{
    private static readonly Color LabelColor = new(0.7f, 0.95f, 0.85f);
    private static readonly Color HoverColor = new(1.0f, 0.95f, 0.5f);
    private static readonly Color NoCardColor = new(0.6f, 0.6f, 0.6f);

    private const float CardIconSize = 20;

    private readonly CardGeneratedEvent _entry;
    private readonly Action<CardModel> _openInspect;

    public CardGeneratedRow(CardGeneratedEvent entry, Action<CardModel> openInspect)
    {
        _entry = entry;
        _openInspect = openInspect;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 4);
        MouseFilter = MouseFilterEnum.Stop;
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

        AddChild(new Label { Text = "    " });

        var glyph = new Label { Text = "\u2795" };
        glyph.AddThemeColorOverride("font_color", LabelColor);
        AddChild(glyph);

        if (_entry.GeneratedCard is not null)
        {
            var tinyCard = TinyCardFactory.Build(_entry.GeneratedCard, CardIconSize);
            if (tinyCard is not null) AddChild(tinyCard);
        }

        var nameLabel = new Label();
        var shortCardName = NameTruncator.Short(_entry.GeneratedCardName);
        var pileSuffix = _entry.Pile.HasValue ? $" \u2192 {PileLabel(_entry.Pile.Value)}" : "";
        var ownerSuffix = string.IsNullOrEmpty(_entry.OwnerName) ? "" : $" [{_entry.OwnerName}]";
        nameLabel.Text = $"{shortCardName}{pileSuffix}{ownerSuffix}";
        var baseColor = _entry.GeneratedCard is null ? NoCardColor : LabelColor;
        nameLabel.AddThemeColorOverride("font_color", baseColor);
        AddChild(nameLabel);

        var card = _entry.GeneratedCard;

        MouseEntered += () =>
        {
            nameLabel.AddThemeColorOverride("font_color", HoverColor);
            if (card is not null)
                try
                {
                    var hoverTip = new CardHoverTip(card);
                    NHoverTipSet.CreateAndShow(this, hoverTip, HoverTipAlignment.Left);
                }
                catch { }
        };

        MouseExited += () =>
        {
            nameLabel.AddThemeColorOverride("font_color", baseColor);
            try { NHoverTipSet.Remove(this); } catch { }
        };

        GuiInput += (@event) =>
        {
            if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } && card is not null)
            {
                try { NHoverTipSet.Remove(this); } catch { }
                _openInspect(card);
            }
        };

        TreeExiting += () =>
        {
            try { NHoverTipSet.Remove(this); } catch { }
        };
    }

    private static string PileLabel(PileType p) => p switch
    {
        PileType.Hand => "Hand",
        PileType.Draw => "Draw",
        PileType.Discard => "Discard",
        PileType.Exhaust => "Exhaust",
        _ => p.ToString(),
    };
}
