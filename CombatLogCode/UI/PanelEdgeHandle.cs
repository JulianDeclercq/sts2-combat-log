using Godot;

namespace CombatLog.CombatLogCode.UI;

/// <summary>
/// Thin invisible strip on one edge or corner of <see cref="CombatLogPanel"/>. Drag to resize.
/// Right edge is anchored to viewport, so only the left side and the two left corners are draggable.
/// Width changes drive <c>CustomMinimumSize.X</c>; height changes drive <c>OffsetTop</c>/<c>OffsetBottom</c>.
/// </summary>
public partial class PanelEdgeHandle : Control
{
    public enum Edge { Left, Top, Bottom, TopLeft, BottomLeft }

    public const float Thickness = 8f;
    public const float MinWidth = 250f;
    public const float MaxWidth = 900f;
    public const float MinHeight = 200f;

    public Edge Kind { get; init; }

    private CombatLogPanel _panel = null!;
    private bool _dragging;
    private Vector2 _startMouse;
    private float _startWidth;
    private float _startOffsetTop;
    private float _startOffsetBottom;

    public override void _Ready()
    {
        _panel = GetParent<CombatLogPanel>();
        switch (Kind)
        {
            case Edge.Left:
                AnchorLeft = 0; AnchorRight = 0; AnchorTop = 0; AnchorBottom = 1;
                OffsetLeft = 0; OffsetRight = Thickness;
                OffsetTop = Thickness; OffsetBottom = -Thickness;
                MouseDefaultCursorShape = CursorShape.Hsize;
                break;
            case Edge.Top:
                AnchorLeft = 0; AnchorRight = 1; AnchorTop = 0; AnchorBottom = 0;
                OffsetLeft = Thickness; OffsetRight = 0;
                OffsetTop = 0; OffsetBottom = Thickness;
                MouseDefaultCursorShape = CursorShape.Vsize;
                break;
            case Edge.Bottom:
                AnchorLeft = 0; AnchorRight = 1; AnchorTop = 1; AnchorBottom = 1;
                OffsetLeft = Thickness; OffsetRight = 0;
                OffsetTop = -Thickness; OffsetBottom = 0;
                MouseDefaultCursorShape = CursorShape.Vsize;
                break;
            case Edge.TopLeft:
                AnchorLeft = 0; AnchorRight = 0; AnchorTop = 0; AnchorBottom = 0;
                OffsetLeft = 0; OffsetRight = Thickness;
                OffsetTop = 0; OffsetBottom = Thickness;
                MouseDefaultCursorShape = CursorShape.Fdiagsize;
                break;
            case Edge.BottomLeft:
                AnchorLeft = 0; AnchorRight = 0; AnchorTop = 1; AnchorBottom = 1;
                OffsetLeft = 0; OffsetRight = Thickness;
                OffsetTop = -Thickness; OffsetBottom = 0;
                MouseDefaultCursorShape = CursorShape.Bdiagsize;
                break;
        }
        MouseFilter = MouseFilterEnum.Stop;
    }

    public override void _GuiInput(InputEvent ev)
    {
        switch (ev)
        {
            case InputEventMouseButton mb when mb.ButtonIndex == MouseButton.Left:
                if (mb.Pressed)
                {
                    _dragging = true;
                    _startMouse = GetGlobalMousePosition();
                    _startWidth = _panel.CustomMinimumSize.X;
                    _startOffsetTop = _panel.OffsetTop;
                    _startOffsetBottom = _panel.OffsetBottom;
                }
                else
                {
                    _dragging = false;
                }
                AcceptEvent();
                break;

            case InputEventMouseMotion when _dragging:
                Apply(GetGlobalMousePosition());
                AcceptEvent();
                break;
        }
    }

    private void Apply(Vector2 mouse)
    {
        if (Kind is Edge.Left or Edge.TopLeft or Edge.BottomLeft)
            ApplyWidth(mouse);
        if (Kind is Edge.Top or Edge.TopLeft)
            ApplyOffsetTop(mouse);
        if (Kind is Edge.Bottom or Edge.BottomLeft)
            ApplyOffsetBottom(mouse);
    }

    private void ApplyWidth(Vector2 mouse)
    {
        // Panel grows leftward — dragging mouse left INCREASES width.
        var w = Math.Clamp(_startWidth + (_startMouse.X - mouse.X), MinWidth, MaxWidth);
        _panel.CustomMinimumSize = new Vector2(w, _panel.CustomMinimumSize.Y);
    }

    private void ApplyOffsetTop(Vector2 mouse)
    {
        var newTop = _startOffsetTop + (mouse.Y - _startMouse.Y);
        var span = AnchorSpanY();
        var height = span + _panel.OffsetBottom - newTop;
        if (height < MinHeight)
            newTop = span + _panel.OffsetBottom - MinHeight;
        _panel.OffsetTop = newTop;
    }

    private void ApplyOffsetBottom(Vector2 mouse)
    {
        var newBot = _startOffsetBottom + (mouse.Y - _startMouse.Y);
        var span = AnchorSpanY();
        var height = span + newBot - _panel.OffsetTop;
        if (height < MinHeight)
            newBot = MinHeight + _panel.OffsetTop - span;
        _panel.OffsetBottom = newBot;
    }

    private float AnchorSpanY()
    {
        var vh = GetViewport().GetVisibleRect().Size.Y;
        return (_panel.AnchorBottom - _panel.AnchorTop) * vh;
    }
}
