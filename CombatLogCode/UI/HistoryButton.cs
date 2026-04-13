using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace CombatLog.CombatLogCode.UI;

public partial class HistoryButton : TextureRect
{
    private const string TexturePath = "res://images/relics/history_course.png";
    private const float HoverScale = 1.15f;
    private const float TweenDuration = 0.1f;

    private Tween? _tween;

    public override void _Ready()
    {
        var tex = GD.Load<Texture2D>(TexturePath);
        if (tex is null)
        {
            GD.PrintErr($"[CombatLog] HistoryButton: texture not found at '{TexturePath}'.");
            return;
        }

        Texture = tex;
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        MouseFilter = MouseFilterEnum.Stop;
        SelfModulate = new Color(1.6f, 1.6f, 1.6f, 1);

        // Scale from center
        PivotOffset = Size / 2;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        GuiInput += OnGuiInput;
    }

    private void OnMouseEntered()
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", new Vector2(HoverScale, HoverScale), TweenDuration)
            .SetEase(Tween.EaseType.Out);

        // Read hover tip lazily from sibling NDiscardPileButton (it's populated by now)
        var discardBtn = GetParent()
            ?.FindChildren("*", recursive: false, owned: false)
            .OfType<NDiscardPileButton>()
            .FirstOrDefault();

        if (discardBtn is not null)
        {
            var hoverTip = Traverse.Create(discardBtn).Field("_hoverTip").GetValue<IHoverTip>();
            if (hoverTip is not null)
                NHoverTipSet.CreateAndShow(this, hoverTip, HoverTipAlignment.Left);
        }
    }

    private void OnMouseExited()
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", Vector2.One, TweenDuration)
            .SetEase(Tween.EaseType.Out);

        NHoverTipSet.Remove(this);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            CombatLogPanel.Instance?.Toggle();
        }
    }
}
