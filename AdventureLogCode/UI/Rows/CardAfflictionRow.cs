using AdventureLog.AdventureLogCode.Events;
using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace AdventureLog.AdventureLogCode.UI.Rows;

public partial class CardAfflictionRow : HBoxContainer
{
    private static readonly Color LabelColor = new(0.95f, 0.65f, 0.4f);
    private static readonly Color HoverColor = new(1.0f, 0.95f, 0.5f);
    private static readonly Color NoCardColor = new(0.6f, 0.6f, 0.6f);

    private const float CardIconSize = 20;

    private readonly CardAfflictionEvent _entry;
    private readonly Action<CardModel> _openInspect;

    public CardAfflictionRow(CardAfflictionEvent entry, Action<CardModel> openInspect)
    {
        _entry = entry;
        _openInspect = openInspect;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 4);
        MouseFilter = MouseFilterEnum.Stop;
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

        if (_entry.AfflictedCard is not null)
        {
            var tinyCard = TinyCardFactory.Build(_entry.AfflictedCard, CardIconSize);
            if (tinyCard is not null) AddChild(tinyCard);
        }

        var nameLabel = new Label();
        var shortCardName = NameTruncator.Short(_entry.AfflictedCardName);
        var nameText = string.IsNullOrEmpty(_entry.OwnerName)
            ? $"{shortCardName} afflicted ({_entry.AfflictionTitle})"
            : $"{shortCardName} afflicted ({_entry.AfflictionTitle}) [{_entry.OwnerName}]";
        nameLabel.Text = nameText;
        var baseColor = _entry.AfflictedCard is null ? NoCardColor : LabelColor;
        nameLabel.AddThemeColorOverride("font_color", baseColor);
        AddChild(nameLabel);

        var card = _entry.AfflictedCard;

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
}
