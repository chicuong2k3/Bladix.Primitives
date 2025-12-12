namespace Bladix.Primitives.Components.Tooltip;

/// <summary>
/// Specifies the sticky behavior on the align axis.
/// </summary>
public enum TooltipSticky
{
    /// <summary>
    /// Keep content in boundary as long as trigger is at least partially in boundary.
    /// </summary>
    Partial,
    
    /// <summary>
    /// Keep content in boundary regardless of trigger position.
    /// </summary>
    Always
}
