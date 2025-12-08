namespace Bladix.Primitives.Components.Tabs;

/// <summary>
/// Determines how tabs are activated when navigating with keyboard
/// </summary>
public enum ActivationMode
{
    /// <summary>
    /// Tabs are activated automatically when focused via keyboard navigation
    /// </summary>
    Automatic,
    
    /// <summary>
    /// Tabs must be activated manually (e.g., with Enter or Space) after being focused
    /// </summary>
    Manual
}
