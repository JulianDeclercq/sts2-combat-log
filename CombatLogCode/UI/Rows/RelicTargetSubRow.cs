using Godot;

namespace CombatLog.CombatLogCode.UI.Rows;

public partial class RelicTargetSubRow : HBoxContainer
{
    private readonly string _targetName;
    private readonly uint? _combatId;
    private readonly CreatureHighlighter _highlighter;

    public RelicTargetSubRow(string targetName, uint? combatId, CreatureHighlighter highlighter)
    {
        _targetName = targetName;
        _combatId = combatId;
        _highlighter = highlighter;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 4);
        MouseFilter = MouseFilterEnum.Stop;

        AddChild(new Label { Text = "    " });

        var label = new Label();
        label.Text = $"\u2192 {NameTruncator.Short(_targetName)}";
        label.AddThemeColorOverride("font_color", DamageColors.Source);
        AddChild(label);

        MouseEntered += () =>
        {
            label.AddThemeColorOverride("font_color", DamageColors.Hover);
            _highlighter.Highlight(_combatId);
        };

        MouseExited += () =>
        {
            label.AddThemeColorOverride("font_color", DamageColors.Source);
            _highlighter.Clear();
        };
    }
}
