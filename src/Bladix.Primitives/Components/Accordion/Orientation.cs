namespace Bladix.Primitives;

/// <summary>
/// The orientation of the accordion. This affects keyboard navigation behavior.
/// This is used by RovingFocus for keyboard navigation:
/// - Vertical: Up/Down arrows navigate between triggers
/// - Horizontal: Left/Right arrows navigate between triggers
/// 
/// Note: Full keyboard navigation support would require implementing RovingFocus pattern
/// which is not included in this basic port.
/// </summary>
public enum Orientation
{
    Horizontal,
    Vertical
}
