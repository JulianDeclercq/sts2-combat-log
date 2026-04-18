using Godot;

namespace CombatLog.CombatLogCode.UI;

/// <summary>
/// Thin grip on the panel's left edge. Drag to resize.
/// Panel anchors right with GrowDirection.Begin, so width is driven by CustomMinimumSize.X.
/// </summary>
public partial class PanelResizeHandle : Control
{
    public const float HandleWidth = 8f;
    public const float MinWidth = 250f;
    public const float MaxWidth = 900f;

    private CombatLogPanel _panel = null!;
    private bool _dragging;
    private float _dragStartMouseX;
    private float _dragStartWidth;

    public override void _Ready()
    {
        _panel = GetParent<CombatLogPanel>();
        AnchorLeft = 0; AnchorRight = 0;
        AnchorTop = 0; AnchorBottom = 1;
        OffsetLeft = 0; OffsetRight = HandleWidth;
        OffsetTop = 0; OffsetBottom = 0;
        MouseDefaultCursorShape = CursorShape.Hsize;
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
                    _dragStartMouseX = GetGlobalMousePosition().X;
                    _dragStartWidth = _panel.CustomMinimumSize.X;
                }
                else
                {
                    _dragging = false;
                }
                AcceptEvent();
                break;

            case InputEventMouseMotion when _dragging:
                // Panel grows leftward, so dragging mouse left INCREASES width.
                var delta = _dragStartMouseX - GetGlobalMousePosition().X;
                var newWidth = Math.Clamp(_dragStartWidth + delta, MinWidth, MaxWidth);
                _panel.CustomMinimumSize = new Vector2(newWidth, 0);
                AcceptEvent();
                break;
        }
    }
}
