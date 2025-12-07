namespace Bladix.Primitives;

/// <summary>
/// Determines whether one or multiple items can be opened at the same time.
/// </summary>
public enum AccordionType
{
    /// <summary>
    /// Only one accordion item can be open at a time
    /// </summary>
    Single,
    
    /// <summary>
    /// Multiple accordion items can be open at the same time
    /// </summary>
    Multiple
}
