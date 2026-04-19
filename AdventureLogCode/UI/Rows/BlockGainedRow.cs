using AdventureLog.AdventureLogCode.Events;
using Godot;

namespace AdventureLog.AdventureLogCode.UI.Rows;

public partial class BlockGainedRow : HBoxContainer
{
    private static readonly Color BlockColor = new(0.5f, 0.8f, 1.0f);
    private static readonly Color SourceColor = new(0.7f, 0.7f, 0.5f);
    private static readonly Color HoverColor = new(1.0f, 1.0f, 0.6f);

    private readonly BlockGainedEvent _entry;
    private readonly CreatureHighlighter _highlighter;
    private readonly bool _showSource;

    public BlockGainedRow(BlockGainedEvent entry, CreatureHighlighter highlighter, bool showSource = true)
    {
        _entry = entry;
        _highlighter = highlighter;
        _showSource = showSource;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 4);
        MouseFilter = MouseFilterEnum.Stop;
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

        AddChild(new Label { Text = "    " });

        var label = new Label();
        var receiverSuffix = string.IsNullOrEmpty(_entry.OwnerName) ? "" : $" [{_entry.OwnerName}]";
        label.Text = $"+{_entry.Amount} block{receiverSuffix}";
        label.AddThemeColorOverride("font_color", BlockColor);
        AddChild(label);

        if (_showSource && !string.IsNullOrEmpty(_entry.SourceCardName))
        {
            var sourceLabel = new Label();
            sourceLabel.Text = $" (from {_entry.SourceCardName})";
            sourceLabel.AddThemeColorOverride("font_color", SourceColor);
            AddChild(sourceLabel);
        }

        MouseEntered += () =>
        {
            label.AddThemeColorOverride("font_color", HoverColor);
            _highlighter.Highlight(_entry.ReceiverCombatId);
        };

        MouseExited += () =>
        {
            label.AddThemeColorOverride("font_color", BlockColor);
            _highlighter.Clear();
        };

        TreeExiting += () =>
        {
            try { _highlighter.Clear(); } catch { }
        };
    }
}
